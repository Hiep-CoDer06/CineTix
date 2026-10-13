using System.ComponentModel.DataAnnotations;

namespace CineTix.Models;

public class Seat
{
    public int Id { get; set; }

    [Display(Name = "Suất chiếu")]
    public int ShowtimeId { get; set; }

    public Showtime? Showtime { get; set; }

    [Required(ErrorMessage = "Mã ghế là bắt buộc")]
    [StringLength(10)]
    [Display(Name = "Mã ghế")]
    public string SeatCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Loại ghế là bắt buộc")]
    [StringLength(50)]
    [Display(Name = "Loại ghế")]
    public string SeatType { get; set; } = "Thường";

    [Display(Name = "Đã được đặt")]
    public bool IsBooked { get; set; }

    public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
}