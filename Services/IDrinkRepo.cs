using Drinks.API.Entities;
using Drinks.API.ResourceParameters;

namespace Drinks.API.Services;

public interface IDrinkRepo
{
    Task<(IEnumerable<Drink>, PaginationMetadata)> GetAllDrinksAsync(
        DrinksResourceParameters  parameters);
    
    Task<Drink?> GetDrinkByIdAsync(int id);

    
    void CreateDrink(Drink drink);
    
    void DeleteDrink(Drink drink);
    
    Task SaveDrinkAsync();
}