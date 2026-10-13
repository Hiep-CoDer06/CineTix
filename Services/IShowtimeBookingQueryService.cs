using CineTix.Dtos;

namespace CineTix.Services;

public interface IShowtimeBookingQueryService
{
    Task<BookingShowtimeResponse?> GetBookingDataAsync(
        int showtimeId,
        CancellationToken cancellationToken);
}
