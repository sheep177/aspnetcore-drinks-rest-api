using System.ComponentModel.DataAnnotations;

namespace Drinks.API.Models;

public abstract class DrinksForManipulationDto : IValidatableObject
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100)]
    public virtual string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Brand is required")]
    [MaxLength(100)]
    public virtual string Brand { get; set; } = string.Empty;

    [Range(0.01, 1000)]
    public virtual decimal Price { get; set; }

    
    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (Name == Brand)
        {
            yield return new ValidationResult(
                "Name must be different from Brand",
                new[] { nameof(Name), nameof(Brand) }
            );
        }
    }
}