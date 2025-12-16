using AutoMapper;
using Drinks.API.Entities;
using Drinks.API.Models;
using Drinks.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Drinks.API.Controllers;

[ApiController]
[Route("api/drinks")]
[Authorize] 
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
    public async Task<ActionResult<IEnumerable<DrinksDto>>> GetAllDrinks(
        string? searchQuery,
        string? brand,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var (drinks, paginationMetadata) =
            await _repo.GetAllDrinksAsync(searchQuery, brand, pageNumber, pageSize);

        // 分页信息放 Header（标准做法）
        Response.Headers.Add(
            "X-Pagination",
            JsonSerializer.Serialize(paginationMetadata));

        return Ok(_mapper.Map<IEnumerable<DrinksDto>>(drinks));
    }

    // ============================
    // GET /api/drinks/{id}
    // ============================
    [HttpGet("{id}", Name = "GetDrink")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DrinksDto>> GetDrink(int id)
    {
        var entity = await _repo.GetDrinkByIdAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<DrinksDto>(entity));
    }

    // ============================
    // POST /api/drinks
    // ============================
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<DrinksDto>> CreateDrink(
        DrinksForCreationDto drinkForCreation)
    {
        var entity = _mapper.Map<Drink>(drinkForCreation);

        _repo.CreateDrink(entity);
        await _repo.SaveDrinkAsync();

        var dto = _mapper.Map<DrinksDto>(entity);

        return CreatedAtRoute(
            "GetDrink",
            new { id = dto.Id },
            dto);
    }

    // ============================
    // PUT /api/drinks/{id}
    // ============================
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDrink(
        int id,
        DrinksForUpdateDto drinkForUpdate)
    {
        var entity = await _repo.GetDrinkByIdAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        _mapper.Map(drinkForUpdate, entity);
        await _repo.SaveDrinkAsync();

        return NoContent();
    }

    // ============================
    // PATCH /api/drinks/{id}
    // ============================
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PartiallyUpdateDrink(
        int id,
        JsonPatchDocument<DrinksPatchDto> patchDoc)
    {
        var entity = await _repo.GetDrinkByIdAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        var dtoToPatch = _mapper.Map<DrinksPatchDto>(entity);
        patchDoc.ApplyTo(dtoToPatch, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
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
}