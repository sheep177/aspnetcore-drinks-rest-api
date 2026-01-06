using Drinks.API.Helpers;
using Drinks.API.Models;
using Drinks.API.ResourceParameters;

public interface IDrinkService
{
    Task<PagedList<DrinksDto>> GetAllDrinksAsync(
        DrinksResourceParameters parameters);

    Task<(DrinksDto? drink, string etag)> GetDrinkWithETagAsync(int id);

    Task<bool> DrinkExistsAsync(int id);

    Task<DrinksDto> CreateDrinkAsync(DrinksForCreationDto input);

    Task<bool> DeleteDrinkAsync(int id);

    Task<bool> UpdateDrinkAsync(
        int id,
        DrinksForUpdateDto input,
        string? ifMatch);
    
    Task<bool> PatchDrinkAsync(
        int id,
        DrinksPatchDto patchDto,
        string? ifMatch);
}