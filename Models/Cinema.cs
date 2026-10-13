using System.ComponentModel.DataAnnotations;

namespace CineTix.Models;

public class Cinema
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên rạp là bắt buộc")]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
    [StringLength(300)]
    public string Address { get; set; } = string.Empty;

    [StringLength(100)]
    public string? City { get; set; }

    [Phone]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }
}