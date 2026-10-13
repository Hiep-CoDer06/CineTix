using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CineTix.Models;

public class FoodCombo
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên combo là bắt buộc")]
    [StringLength(100)]
    [Display(Name = "Tên combo")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mô tả combo là bắt buộc")]
    [StringLength(250)]
    [Display(Name = "Mô tả")]
    public string Description { get; set; } = string.Empty;

    [Precision(18, 2)]
    [Range(0, 1000000)]
    [Display(Name = "Giá combo")]
    public decimal Price { get; set; }

    [Display(Name = "Đang bán")]
    public bool IsAvailable { get; set; } = true;

    public ICollection<BookingCombo> BookingCombos { get; set; } = new List<BookingCombo>();
}
