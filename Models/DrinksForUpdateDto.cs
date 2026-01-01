using System.ComponentModel.DataAnnotations;

namespace Drinks.API.Models;

public class DrinksForUpdateDto:  DrinksForManipulationDto
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "Brand is required")]
    public string Brand { get; set; } = string.Empty;
    
}