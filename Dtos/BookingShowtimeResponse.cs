namespace CineTix.Dtos;

/// <summary>
/// Dữ liệu một suất chiếu cần thiết khi người dùng chọn ghế.
/// </summary>
public sealed record BookingShowtimeResponse(
    int ShowtimeId,
    string Description,
    decimal TicketPrice,
    IReadOnlyList<AvailableSeatResponse> AvailableSeats);
