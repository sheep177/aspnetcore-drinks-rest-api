using Drinks.API.DbContext;
using Drinks.API.Entities;
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
    public async Task<(IEnumerable<Drink>, PaginationMetadata)> GetAllDrinksAsync(
        string? searchQuery,
        string? brand,
        int pageNumber,
        int pageSize)
    {
        IQueryable<Drink> collection = _context.Drinks;
        
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            searchQuery = searchQuery.Trim();

            collection = collection.Where(d =>
                d.Name.Contains(searchQuery) ||
                (d.Brand != null && d.Brand.Contains(searchQuery)));
        }
        
        if (!string.IsNullOrWhiteSpace(brand))
        {
            brand = brand.Trim();
            collection = collection.Where(d => d.Brand == brand);
        }
        
        var totalItemCount = await collection.CountAsync();
        
        pageSize = Math.Min(pageSize, 20); 
        collection = collection
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
        
        var drinks = await collection.ToListAsync();
        
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
        return await _context.Drinks.FirstOrDefaultAsync(d => d.Id == id);
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
    public async Task<bool> SaveDrinkAsync()
    {
        return await _context.SaveChangesAsync() >= 0;
    }

    // ============================
    // DELETE
    // ============================
    public void DeleteDrink(Drink drink)
    {
        _context.Drinks.Remove(drink);
    }
}