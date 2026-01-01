using Drinks.API.Entities;

namespace Drinks.API.Models;

public class DrinksForCreationDto: DrinksForManipulationDto
{
    
    public ICollection<IngredientForCreationDto> Ingredients { get; set; }
        = new List<IngredientForCreationDto>();
}