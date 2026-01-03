using AutoMapper;
using Drinks.API.Entities;
using Drinks.API.Models;
using Drinks.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Drinks.API.ResourceParameters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace Drinks.API.Controllers;

[ApiController]
[Route("api/drinks")]
[Authorize]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class DrinkController : ControllerBase
{
    private readonly IDrinkRepo _repo;
    private readonly IMapper _mapper;

    public DrinkController(IDrinkRepo repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    // ============================
    // GET /api/drinks
    // ============================
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DrinksDto>>> GetAllDrinks(//actionresult允许返回OK，NotFound，BadRequest
       [FromQuery]DrinksResourceParameters parameters)//这里如果不写的话就默认用我repo的！string。IsNullOrWhiteSpace哪个地方
    {
        parameters.PageSize = Math.Min(parameters.PageSize, 20);//这是serverside hard cap，防止user乱写，防止dos和naive client

        var (drinks, paginationMetadata) =
            await _repo.GetAllDrinksAsync(parameters);

        Response.Headers.Add(
            "X-Pagination",
            JsonSerializer.Serialize(paginationMetadata));//这里不需要主动把param的信息赛道paginationmetadata，APIcontroller自动帮你做
                                                                //简单类型例如string，int bool默认从query strin取值，如果没传就用默认值

        return Ok(_mapper.Map<IEnumerable<DrinksDto>>(drinks));
    }

    [HttpGet("{id}", Name = "GetDrink")]//apicontroller自动从param绑定，只要param和这个同名就自动绑定默认成FromRoute
    //如果出现同名就是route和query的话，优先级是Route>Query>Body>Header/Form
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DrinksDto>> GetDrink(int id)
    {
        var entity = await _repo.GetDrinkByIdAsync(id);
        if (entity == null) return NotFound();
        return Ok(_mapper.Map<DrinksDto>(entity));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<DrinksDto>> CreateDrink(DrinksForCreationDto input)
    {
        var entity = _mapper.Map<Drink>(input);

        _repo.CreateDrink(entity);//是同步方法，不涉及IO，不访问数据库，所以不会进行异步操作。entity 被 标记为 Added
        await _repo.SaveDrinkAsync();//EF Core- 色换个昵称SQL，插入数据库，回填entity。Id

        var output = _mapper.Map<DrinksDto>(entity);

        return CreatedAtRoute(//CreatedAtRoute(string routeName, object routeValues, object value)
            "GetDrink",
            new { id = entity.Id }, //实际期望一个“包含路由参数名和值”的<<对象>>所以要new,就不能写id = entity.Id，因为这只是个赋值
            output);
    }

    // ============================
    // PUT /api/drinks/{id}
    // ============================
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> UpsertDrink(
        int id,
        DrinksForUpdateDto input)
    {
        var entity = await _repo.GetDrinkByIdAsync(id);

        // 情况 1：资源不存在 → CREATE
        if (entity == null)
        {
            var newDrink = _mapper.Map<Drink>(input);
            newDrink.Id = id; // 关键：URI 决定 ID

            _repo.CreateDrink(newDrink);
            await _repo.SaveDrinkAsync();

            return CreatedAtRoute(
                "GetDrink",
                new { id = newDrink.Id },
                _mapper.Map<DrinksDto>(newDrink));
        }

        // 情况 2：资源存在 → UPDATE
        _mapper.Map(input, entity);
        await _repo.SaveDrinkAsync();

        return NoContent();
    }

    // ============================
    // PATCH /api/drinks/{id}
    // ============================
    [HttpPatch("{id}")]
    [Consumes("application/json-patch+json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PartiallyUpdateDrink(
        int id,
        JsonPatchDocument<DrinksPatchDto> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        var entity = await _repo.GetDrinkByIdAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        var dtoToPatch = _mapper.Map<DrinksPatchDto>(entity);

        patchDoc.ApplyTo(dtoToPatch, ModelState);

        if (!TryValidateModel(dtoToPatch))
        {
            return ValidationProblem(ModelState);
        }

        _mapper.Map(dtoToPatch, entity);
        await _repo.SaveDrinkAsync();

        return NoContent();
    }

    // ============================
    // DELETE /api/drinks/{id}
    // ============================
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDrink(int id)
    {
        var entity = await _repo.GetDrinkByIdAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        _repo.DeleteDrink(entity);
        await _repo.SaveDrinkAsync();

        return NoContent();
    }
    
    [HttpOptions]
    public IActionResult GetAuthorsOptions()
    {
        Response.Headers.Add("Allow", "GET, HEAD, POST, OPTIONS");
        return Ok();
    }
    
    public override ActionResult ValidationProblem(
        ModelStateDictionary modelStateDictionary)
    {
        var options = HttpContext.RequestServices
            .GetRequiredService<IOptions<ApiBehaviorOptions>>();

        return (ActionResult)options.Value
            .InvalidModelStateResponseFactory(ControllerContext);
    }
}