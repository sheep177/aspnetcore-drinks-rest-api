namespace Drinks.API.Entities;

public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } =  null!;
    public decimal Amount { get; set; }
    public string Unit { get; set; } = null!;
    
    public int DrinkId { get; set; }
    public Drink Drink { get; set; } = null!;
    
}