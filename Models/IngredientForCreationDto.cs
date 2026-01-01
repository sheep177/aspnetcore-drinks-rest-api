using System.ComponentModel.DataAnnotations;
using Drinks.API.Entities;

namespace Drinks.API.Models;

public class IngredientForCreationDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, 1000)]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(20)]
    public string Unit { get; set; } = string.Empty;
}