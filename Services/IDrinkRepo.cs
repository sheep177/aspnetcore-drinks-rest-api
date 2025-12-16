using Drinks.API.Entities;

namespace Drinks.API.Services;

public interface IDrinkRepo
{
    Task<(IEnumerable<Drink>, PaginationMetadata)> GetAllDrinksAsync(
        string? searchQuery,
        string? brand,
        int pageNumber,
        int pageSize);
    
    Task<Drink?> GetDrinkByIdAsync(int id);

    
    void CreateDrink(Drink drink);
    
    void DeleteDrink(Drink drink);
    
    Task<bool> SaveDrinkAsync();
}