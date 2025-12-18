using AutoMapper;
using Drinks.API.Entities;
using Drinks.API.Models;
using Drinks.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Drinks.API.Controllers;

/// <summary>
/// Provides CRUD operations for drinks resources.
/// All endpoints require authentication.
/// </summary>
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

    /// <summary>
    /// Retrieves a paginated list of drinks.
    /// Supports optional search and brand filtering.
    /// </summary>
    /// <param name="searchQuery">
    /// Optional search keyword matched against drink name or brand.
    /// </param>
    /// <param name="brand">
    /// Optional brand filter (exact match).
    /// </param>
    /// <param name="pageNumber">
    /// Page number (starting from 1).
    /// </param>
    /// <param name="pageSize">
    /// Number of items per page (max 20).
    /// </param>
    /// <returns>
    /// A paginated list of drinks with pagination metadata in response headers.
    /// </returns>
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

        
        Response.Headers.Add(
            "X-Pagination",
            JsonSerializer.Serialize(paginationMetadata));

        return Ok(_mapper.Map<IEnumerable<DrinksDto>>(drinks));
    }

    /// <summary>
    /// Retrieves a single drink by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the drink.</param>
    /// <returns>The requested drink.</returns>
    /// <response code="200">Returns the drink.</response>
    /// <response code="404">If the drink does not exist.</response>
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

    /// <summary>
    /// Create a new drink
    /// </summary>
    /// <param name="drinkForCreation">The drink details</param>
    /// <returns>The newly created drink</returns>
    /// <response code="201">Drink created successfully.</response>
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

    /// <summary>
    /// Partially updates a drink using JSON Patch.
    /// </summary>
    /// <param name="id">The ID of the drink.</param>
    /// <param name="patchDoc">
    /// JSON Patch document describing changes to apply.
    /// </param>
    /// <response code="204">Update successful.</response>
    /// <response code="404">Drink not found.</response>
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

    /// <summary>
    /// Partially updates a drink using JSON Patch.
    /// </summary>
    /// <param name="id">The ID of the drink.</param>
    /// <param name="patchDoc">
    /// JSON Patch document describing changes to apply.
    /// </param>
    /// <response code="204">Update successful.</response>
    /// <response code="404">Drink not found.</response>
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