namespace Drinks.API.Models;

public class IngredientDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Unit { get; set; } = null!;
}