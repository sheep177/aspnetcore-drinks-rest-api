using Drinks.API.DbContext;
using Drinks.API.Entities;
using Drinks.API.ResourceParameters;
using Microsoft.EntityFrameworkCore;

namespace Drinks.API.Services;

public class DrinkRepo : IDrinkRepo
{
    private readonly DrinkInfoContext _context;

    public DrinkRepo(DrinkInfoContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // ============================
    // GET ALL（Search + Filter + Paging）
    // ============================
    public async Task<(IEnumerable<Drink>, PaginationMetadata)> 
        GetAllDrinksAsync(DrinksResourceParameters parameters)
    {
        var collection = _context.Drinks
            .Include(d => d.Ingredients)
            .AsQueryable();

        var searchQuery = parameters.SearchQuery?.Trim();
        var brand = parameters.Brand?.Trim();
        var pageSize = Math.Min(parameters.PageSize, 20);
        var pageNumber = parameters.PageNumber;

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            collection = collection.Where(d =>
                d.Name.Contains(searchQuery) ||
                d.Brand.Contains(searchQuery));
        }

        if (!string.IsNullOrWhiteSpace(brand))
        {
            collection = collection.Where(d => d.Brand == brand);
        }

        var totalItemCount = await collection.CountAsync();

        var drinks = await collection
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var metadata = new PaginationMetadata(
            totalItemCount,
            pageSize,
            pageNumber);

        return (drinks, metadata);
    }

    // ============================
    // GET BY ID
    // ============================
    public async Task<Drink?> GetDrinkByIdAsync(int id)
    {
        return await _context.Drinks.Include(d => d.Ingredients).FirstOrDefaultAsync(d => d.Id == id);
    }

    // ============================
    // CREATE
    // ============================
    public void CreateDrink(Drink drink)
    {
        _context.Drinks.Add(drink);
    }

    // ============================
    // UPDATE / DELETE 共用 Save
    // ============================
    public async Task SaveDrinkAsync()
    {
       await _context.SaveChangesAsync();
    }

    // ============================
    // DELETE
    // ============================
    public void DeleteDrink(Drink drink)
    {
        _context.Drinks.Remove(drink);
    }
}