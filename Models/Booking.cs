using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CineTix.Models;

public class Booking
{
    public int Id { get; set; }

    [StringLength(450)]
    public string? UserId { get; set; }
    public IdentityUser? User { get; set; }

    [Display(Name = "Suất chiếu")]
    public int ShowtimeId { get; set; }
    public Showtime? Showtime { get; set; }

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

    [Display(Name = "Thời gian đặt")]
    public DateTime BookedAt { get; set; } = DateTime.Now;

    [Precision(18, 2)]
    [Display(Name = "Tổng tiền")]
    public decimal TotalAmount { get; set; }

    [StringLength(30)]
    [Display(Name = "Trạng thái")]
    public string Status { get; set; } = "Đã đặt";

    public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
    public ICollection<BookingCombo> BookingCombos { get; set; } = new List<BookingCombo>();
}
