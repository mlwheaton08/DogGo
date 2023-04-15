using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DogGo.Models;

public class Owner
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Dawg, add a name.")]
    [MaxLength(50)]
    public string Name { get; set; }

    [EmailAddress]
    [Required]
    public string Email { get; set; }

    [Required]
    [StringLength(55, MinimumLength = 6)]
    public string Address { get; set; }

    [Phone]
    [DisplayName("Phone Number")]
    public string Phone { get; set; }

    [Required]
    public int NeighborhoodId { get; set; }
    public Neighborhood Neighborhood { get; set; }
    public List<Dog> Dogs { get; set; } = new List<Dog>();
}