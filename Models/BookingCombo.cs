using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CineTix.Models;

public class BookingCombo
{
    public int Id { get; set; }

    public int BookingId { get; set; }
    public Booking? Booking { get; set; }

    public int FoodComboId { get; set; }
    public FoodCombo? FoodCombo { get; set; }

    [Required]
    [StringLength(100)]
    public string ComboName { get; set; } = string.Empty;

    [Range(1, 10)]
    public int Quantity { get; set; }

    [Precision(18, 2)]
    public decimal UnitPrice { get; set; }
}
