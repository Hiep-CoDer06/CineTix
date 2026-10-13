namespace CineTix.Services;

/// <summary>
/// Quy tắc giá ghế thống nhất cho giao diện đặt vé và khi lưu đơn.
/// Giá của suất chiếu là giá ghế thường; ghế VIP và ghế đôi có phụ thu.
/// </summary>
public static class SeatPricing
{
    public static decimal GetPrice(decimal standardSeatPrice, string? seatType)
    {
        var normalizedType = seatType?.Trim().ToLowerInvariant() ?? string.Empty;

        if (normalizedType.Contains("đôi") || normalizedType.Contains("doi") || normalizedType.Contains("couple"))
        {
            // Ghế đôi dành cho hai khách, có không gian riêng và phụ thu dịch vụ.
            return standardSeatPrice * 2 + 30_000m;
        }

        if (normalizedType.Contains("vip"))
        {
            return standardSeatPrice + 20_000m;
        }

        return standardSeatPrice;
    }
}
