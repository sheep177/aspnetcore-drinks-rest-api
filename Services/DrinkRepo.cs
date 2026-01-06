using Drinks.API.DbContext;
using Drinks.API.Entities;
using Drinks.API.Helpers;
using Drinks.API.ResourceParameters;
using Microsoft.EntityFrameworkCore;

namespace Drinks.API.Services;

public class DrinkRepo : IDrinkRepo
{
    private readonly DrinkInfoContext _context;

    public DrinkRepo(DrinkInfoContext context)
    {
        _context = context;
    }

    public async Task<PagedList<Drink>> GetAllDrinksAsync(
        DrinksResourceParameters parameters)
    {
        var collection = _context.Drinks
            .Include(d => d.Ingredients)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.SearchQuery))
        {
            var search = parameters.SearchQuery.Trim();
            collection = collection.Where(d =>
                d.Name.Contains(search) ||
                d.Brand.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(parameters.Brand))
        {
            var brand = parameters.Brand.Trim();
            collection = collection.Where(d => d.Brand == brand);
        }
        // 👇 sorting（在 paging 前）
        collection = collection.ApplySort(
            parameters.OrderBy,
            DrinkPropertyMapping.Mapping);

        var pageSize = Math.Min(parameters.PageSize, 20);
        var pageNumber = parameters.PageNumber < 1 ? 1 : parameters.PageNumber;

        return await PagedList<Drink>.CreateAsync(
            collection,
            pageNumber,
            pageSize
        );
    }

    public async Task<Drink?> GetDrinkByIdAsync(int id)
    {
        return await _context.Drinks
            .Include(d => d.Ingredients)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public void CreateDrink(Drink drink)
    {
        _context.Drinks.Add(drink);
    }

    public async Task<IEnumerable<Drink>> GetDrinksByIdsAsync(IEnumerable<int> ids)
    {
        return await _context.Drinks
            .Where(d => ids.Contains(d.Id))
            .Include(d => d.Ingredients)
            .ToListAsync();
    }
    public void DeleteDrink(Drink drink)
    {
        _context.Drinks.Remove(drink);
    }
    

    public async Task SaveDrinkAsync()
    {
        await _context.SaveChangesAsync();
    }
}