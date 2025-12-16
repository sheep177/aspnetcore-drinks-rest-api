using Microsoft.Extensions.Options;

namespace Drinks.API.DbContext;
using Microsoft.EntityFrameworkCore;
using Drinks.API.Entities;

public class DrinkInfoContext: DbContext
{
    public DrinkInfoContext(DbContextOptions<DrinkInfoContext> options) : base(options)
    {
        
    }
    
    public DbSet<Drink> Drinks { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Drink>().HasData(
            new Drink { Id = 1, Name = "Cola", Brand = "Coke", Price = 3.5m },
            new Drink { Id = 2, Name = "Green Tea", Brand = "ItoEn", Price = 4.0m }
        );
    }
}