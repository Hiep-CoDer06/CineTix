using System.ComponentModel.DataAnnotations;

namespace CineTix.Models;

public class Movie
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên phim là bắt buộc")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Range(1, 500, ErrorMessage = "Thời lượng phải từ 1 đến 500 phút")]
    public int DurationMinutes { get; set; }

    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; }

    [StringLength(500)]
    public string? PosterUrl { get; set; }

    public bool IsNowShowing { get; set; }

    [Display(Name = "Phim hot")]
    public bool IsHot { get; set; }

    [Display(Name = "Phim kinh điển")]
    public bool IsClassic { get; set; }

    [Display(Name = "Ưu tiên đầu danh sách")]
    public bool IsFeatured { get; set; }

    [Display(Name = "Thể loại")]
    public int? GenreId { get; set; }

    public Genre? Genre { get; set; }
}
