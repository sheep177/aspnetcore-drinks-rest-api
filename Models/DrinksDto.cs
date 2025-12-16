namespace Drinks.API.Models;

public class DrinksDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } =  string.Empty;
    public decimal Price { get; set; }
}