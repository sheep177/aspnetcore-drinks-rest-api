using Drinks.API.Entities;

namespace Drinks.API.Models;

public class IngredientForCreationDto
{
    public string Name { get; set; } =  null!;
    public decimal Amount { get; set; }
    public string Unit { get; set; } =  null!;
    public IEnumerable<IngredientDto> Ingredients { get; set; } =  new List<IngredientDto>();
}