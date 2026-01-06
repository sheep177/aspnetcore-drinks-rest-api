using Drinks.API.Models;
using Drinks.API.ResourceParameters;
using Drinks.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Drinks.API.Controllers;

[ApiController]
[Route("api/drinks")]
[Authorize]
[Produces("application/json")]
public class DrinkController : ControllerBase
{
    private readonly IDrinkService _service;

    public DrinkController(IDrinkService service)
    {
        _service = service;
    }

    // ============================
    // GET /api/drinks
    // ============================
    [HttpGet(Name = "GetDrinks")]
    public async Task<ActionResult<IEnumerable<DrinksDto>>> GetAllDrinks(
        [FromQuery] DrinksResourceParameters parameters)
    {
        var pagedDrinks = await _service.GetAllDrinksAsync(parameters);

        var paginationMetadata = new
        {
            pagedDrinks.TotalCount,
            pagedDrinks.PageSize,
            pagedDrinks.CurrentPage,
            pagedDrinks.TotalPages,
            pagedDrinks.HasPrevious,
            pagedDrinks.HasNext
        };

        Response.Headers.Add(
            "X-Pagination",
            JsonSerializer.Serialize(paginationMetadata));

        return Ok(pagedDrinks);
    }

    // ============================
    // GET /api/drinks/{id} (ETag)
    // ============================
    [HttpGet("{id}", Name = "GetDrink")]
    public async Task<ActionResult<DrinksDto>> GetDrink(int id)
    {
        var (drink, etag) = await _service.GetDrinkWithETagAsync(id);
        if (drink == null)
        {
            return NotFound();
        }

        // If-None-Match handling
        if (Request.Headers.IfNoneMatch.Any(h => h == etag))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        Response.Headers.ETag = etag;
        return Ok(drink);
    }

    // ============================
    // HEAD /api/drinks/{id}
    // ============================
    [HttpHead("{id}")]
    public async Task<IActionResult> HeadDrink(int id)
    {
        var exists = await _service.DrinkExistsAsync(id);
        if (!exists)
        {
            return NotFound();
        }

        return Ok();
    }

    // ============================
    // POST /api/drinks
    // ============================
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<DrinksDto>> CreateDrink(
        DrinksForCreationDto input)
    {
        var created = await _service.CreateDrinkAsync(input);

        return CreatedAtRoute(
            "GetDrink",
            new { id = created.Id },
            created);
    }

    // ============================
    // PUT /api/drinks/{id} (If-Match)
    // ============================
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
    public async Task<IActionResult> UpdateDrink(
        int id,
        DrinksForUpdateDto input)
    {
        try
        {
            var updated = await _service.UpdateDrinkAsync(
                id,
                input,
                Request.Headers.IfMatch.FirstOrDefault());

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch
        {
            // 并发冲突
            return StatusCode(StatusCodes.Status412PreconditionFailed);
        }
    }
    
    // ============================
// PATCH /api/drinks/{id}
// ============================
    [HttpPatch("{id}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
    public async Task<IActionResult> PatchDrink(
        int id,
        DrinksPatchDto input)
    {
        try
        {
            var patched = await _service.PatchDrinkAsync(
                id,
                input,
                Request.Headers.IfMatch.FirstOrDefault());

            if (!patched)
                return NotFound();

            return NoContent();
        }
        catch
        {
            return StatusCode(StatusCodes.Status412PreconditionFailed);
        }
    }

    // ============================
    // DELETE /api/drinks/{id}
    // ============================
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDrink(int id)
    {
        var deleted = await _service.DeleteDrinkAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    // ============================
    // OPTIONS
    // ============================
    [HttpOptions]
    public IActionResult GetDrinksOptions()
    {
        Response.Headers.Add("Allow", "GET, HEAD, POST, PUT, DELETE, OPTIONS");
        return Ok();
    }
}