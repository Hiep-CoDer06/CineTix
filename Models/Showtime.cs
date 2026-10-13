using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace CineTix.Models;

public class Showtime
{
    public int Id { get; set; }

    [Display(Name = "Phim")]
    public int MovieId { get; set; }
    public Movie? Movie { get; set; }

    [Display(Name = "Rạp")]
    public int CinemaId { get; set; }
    public Cinema? Cinema { get; set; }

    [Required(ErrorMessage = "Thời gian chiếu là bắt buộc")]
    [Display(Name = "Thời gian chiếu")]
    public DateTime StartTime { get; set; }

    [Required(ErrorMessage = "Tên phòng chiếu là bắt buộc")]
    [StringLength(50)]
    [Display(Name = "Phòng chiếu")]
    public string RoomName { get; set; } = string.Empty;

    [Precision(18, 2)]
    [Range(0, 1000000, ErrorMessage = "Giá vé không hợp lệ")]
    [Display(Name = "Giá vé")]
    public decimal TicketPrice { get; set; }
    public ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}