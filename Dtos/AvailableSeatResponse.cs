namespace CineTix.Dtos;

/// <summary>
/// Một ghế còn trống được trả về cho giao diện đặt vé.
/// </summary>
public sealed record AvailableSeatResponse(
    int Id,
    string SeatCode,
    string SeatType,
    decimal UnitPrice);
