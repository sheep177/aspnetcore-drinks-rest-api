using AutoMapper;
using Drinks.API.Entities;
using Drinks.API.Models;
using Drinks.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drinks.API.Controllers;

[ApiController]
[Route("api/drinkcollections")]
[Authorize]
[Produces("application/json")]
public class DrinkCollectionsController : ControllerBase
{
    private readonly IDrinkRepo _repository;
    private readonly IMapper _mapper;

    public DrinkCollectionsController(IDrinkRepo repository, IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    // GET /api/drinkcollections/(1,2,3)
    [HttpGet("({ids})", Name = "GetDrinkCollection")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<DrinksDto>>> GetDrinkCollection(
        [FromRoute] IEnumerable<int> ids)
    {
        var drinkEntities = await _repository.GetDrinkByIdAsync(ids);

        // 关键：如果有任何一个 id 不存在，就当作“这个 key 无效”
        if (drinkEntities.Count() != ids.Count())
        {
            return NotFound();
        }

        return Ok(_mapper.Map<IEnumerable<DrinksDto>>(drinkEntities));
    }

    // POST /api/drinkcollections
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<DrinksDto>>> CreateDrinkCollection(
        IEnumerable<DrinksForCreationDto> collection)
    {
        if (collection == null || !collection.Any())
        {
            return BadRequest();
        }

        var drinkEntities = _mapper.Map<IEnumerable<Drink>>(collection);

        foreach (var drink in drinkEntities)
        {
            _repository.CreateDrink(drink);
        }

        await _repository.SaveDrinkAsync();

        var drinksToReturn = _mapper.Map<IEnumerable<DrinksDto>>(drinkEntities);

        // array key：1,2,3
        var ids = string.Join(",", drinksToReturn.Select(d => d.Id));

        return CreatedAtRoute(
            "GetDrinkCollection",
            new { ids },
            drinksToReturn);
    }
}