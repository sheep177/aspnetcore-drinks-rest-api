using Drinks.API.Entities;
using Drinks.API.Helpers;
using Drinks.API.ResourceParameters;

public interface IDrinkRepo
{
    Task<PagedList<Drink>> GetAllDrinksAsync(
        DrinksResourceParameters parameters);

    Task<Drink?> GetDrinkByIdAsync(int id);

    void CreateDrink(Drink drink);
    void DeleteDrink(Drink drink);

    
    Task SaveDrinkAsync();
}