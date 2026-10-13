using System.ComponentModel.DataAnnotations;

namespace CineTix.ViewModels;

public class BookingComboSelectionViewModel
{
    public int FoodComboId { get; set; }

    [Range(0, 10, ErrorMessage = "Mỗi combo chỉ được chọn từ 0 đến 10 phần")]
    public int Quantity { get; set; }
}
