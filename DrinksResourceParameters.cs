namespace Drinks.API.ResourceParameters;

public class DrinksResourceParameters
{
    public string? SearchQuery { get; set; }
    public string? Brand { get; set; }

    public string OrderBy { get; set; } = "Name";
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}