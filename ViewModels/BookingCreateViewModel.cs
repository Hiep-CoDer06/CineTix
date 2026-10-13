using System.ComponentModel.DataAnnotations;
using CineTix.Models;

namespace CineTix.ViewModels;

public class BookingCreateViewModel
{
    [Required(ErrorMessage = "Vui lòng chọn suất chiếu")]
    [Display(Name = "Suất chiếu")]
    public int? ShowtimeId { get; set; }

    [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
    [StringLength(100)]
    [Display(Name = "Tên khách hàng")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Phone]
    [StringLength(20)]
    [Display(Name = "Số điện thoại")]
    public string CustomerPhone { get; set; } = string.Empty;

    [EmailAddress]
    [Display(Name = "Email")]
    public string? CustomerEmail { get; set; }

    public List<int> SelectedSeatIds { get; set; } = new();
    public List<Seat> AvailableSeats { get; set; } = new();
    public List<BookingComboSelectionViewModel> ComboSelections { get; set; } = new();
    public List<FoodCombo> AvailableCombos { get; set; } = new();

    public string? ShowtimeDescription { get; set; }
    public decimal TicketPrice { get; set; }
}
