using System.ComponentModel.DataAnnotations;

namespace CineTix.Models;

public class Genre
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên thể loại là bắt buộc")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}