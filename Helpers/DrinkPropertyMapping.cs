namespace Drinks.API.Helpers;

public static class DrinkPropertyMapping
{
    public static readonly Dictionary<string, string> Mapping =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "name", "Name" },
            { "brand", "Brand" },
            { "price", "Price" }
        };
}