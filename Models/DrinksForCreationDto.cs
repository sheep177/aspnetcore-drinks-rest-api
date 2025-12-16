namespace Drinks.API.Models;

public class DrinksForCreationDto
{
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } =  string.Empty;
    public decimal Price { get; set; }
}