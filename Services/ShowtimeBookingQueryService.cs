using CineTix.Data;
using CineTix.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CineTix.Services;

public sealed class ShowtimeBookingQueryService : IShowtimeBookingQueryService
{
    private readonly CineTixDbContext _context;

    public ShowtimeBookingQueryService(CineTixDbContext context)
    {
        _context = context;
    }

    public async Task<BookingShowtimeResponse?> GetBookingDataAsync(
        int showtimeId,
        CancellationToken cancellationToken)
    {
        var showtime = await _context.Showtimes
            .AsNoTracking()
            .Include(item => item.Movie)
            .Include(item => item.Cinema)
            .FirstOrDefaultAsync(item => item.Id == showtimeId, cancellationToken);

        if (showtime == null)
        {
            return null;
        }

        var availableSeats = await _context.Seats
            .AsNoTracking()
            .Where(seat => seat.ShowtimeId == showtimeId && !seat.IsBooked)
            .OrderBy(seat => seat.SeatCode)
            .Select(seat => new { seat.Id, seat.SeatCode, seat.SeatType })
            .ToListAsync(cancellationToken);

        var seats = availableSeats
            .Select(seat => new AvailableSeatResponse(
                seat.Id,
                seat.SeatCode,
                seat.SeatType,
                SeatPricing.GetPrice(showtime.TicketPrice, seat.SeatType)))
            .ToList();

        var description = $"{showtime.Movie?.Title} - {showtime.Cinema?.Name} - " +
                          showtime.StartTime.ToString("dd/MM/yyyy HH:mm");

        return new BookingShowtimeResponse(
            showtime.Id,
            description,
            showtime.TicketPrice,
            seats);
    }
}
