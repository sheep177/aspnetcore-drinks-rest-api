using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drinks.API.Entities;

public class Drink
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Brand { get; set; } = string.Empty;
    
    [Range(0,10000)]
    public decimal Price { get; set; }
    
    public ICollection<Ingredient> Ingredients { get; set; } = new  List<Ingredient>();
}