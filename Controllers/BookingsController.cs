using CineTix.Data;
using CineTix.Models;
using CineTix.Services;
using CineTix.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
public class BookingsController : Controller
{
    private readonly CineTixDbContext _context;

    public BookingsController(CineTixDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var bookings = await GetAccessibleBookingsQuery()
            .Include(b => b.Showtime!)
                .ThenInclude(s => s.Movie)
            .Include(b => b.Showtime!)
                .ThenInclude(s => s.Cinema)
            .Include(b => b.BookingSeats)
                .ThenInclude(bookingSeat => bookingSeat.Seat)
            .Include(b => b.BookingCombos)
            .OrderByDescending(b => b.BookedAt)
            .ToListAsync();

        return View(bookings);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var booking = await GetBookingWithDetailsAsync(id.Value);

        return booking == null ? NotFound() : View(booking);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? showtimeId)
    {
        showtimeId ??= HttpContext.Session.GetInt32(BookingDraftSession.ShowtimeIdKey);

        if (showtimeId.HasValue)
        {
            HttpContext.Session.SetInt32(BookingDraftSession.ShowtimeIdKey, showtimeId.Value);
        }

        var model = new BookingCreateViewModel { ShowtimeId = showtimeId };
        await PopulateBookingFormAsync(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookingCreateViewModel model)
    {
        await PopulateBookingFormAsync(model);

        model.SelectedSeatIds ??= new List<int>();
        var selectedSeatIds = model.SelectedSeatIds.Distinct().ToList();
        model.ComboSelections ??= new List<BookingComboSelectionViewModel>();
        var selectedCombos = model.ComboSelections
            .Where(selection => selection.Quantity > 0)
            .GroupBy(selection => selection.FoodComboId)
            .Select(group => new { FoodComboId = group.Key, Quantity = group.Sum(item => item.Quantity) })
            .ToList();

        if (selectedSeatIds.Count == 0)
        {
            ModelState.AddModelError(nameof(model.SelectedSeatIds), "Vui lòng chọn ít nhất một ghế");
        }

        if (selectedCombos.Any(combo => combo.Quantity > 10))
        {
            ModelState.AddModelError(nameof(model.ComboSelections), "Mỗi combo chỉ được chọn tối đa 10 phần");
        }

        if (!ModelState.IsValid || model.ShowtimeId == null)
        {
            return View(model);
        }

        HttpContext.Session.SetInt32(BookingDraftSession.ShowtimeIdKey, model.ShowtimeId.Value);

        var showtime = await _context.Showtimes
            .FirstOrDefaultAsync(s => s.Id == model.ShowtimeId.Value);

        if (showtime == null)
        {
            ModelState.AddModelError(nameof(model.ShowtimeId), "Suất chiếu không tồn tại");
            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var seats = await _context.Seats
            .Where(s => selectedSeatIds.Contains(s.Id)
                        && s.ShowtimeId == showtime.Id
                        && !s.IsBooked)
            .ToListAsync();

        if (seats.Count != selectedSeatIds.Count)
        {
            ModelState.AddModelError(nameof(model.SelectedSeatIds),
                "Một hoặc nhiều ghế đã được đặt hoặc không thuộc suất chiếu này");
            await PopulateBookingFormAsync(model);
            return View(model);
        }

        var comboIds = selectedCombos.Select(combo => combo.FoodComboId).ToList();
        var combosById = await _context.FoodCombos
            .Where(combo => comboIds.Contains(combo.Id) && combo.IsAvailable)
            .ToDictionaryAsync(combo => combo.Id);

        if (combosById.Count != selectedCombos.Count)
        {
            ModelState.AddModelError(nameof(model.ComboSelections), "Có combo không còn được bán. Vui lòng chọn lại.");
            await PopulateBookingFormAsync(model);
            return View(model);
        }

        var comboTotal = selectedCombos.Sum(combo =>
            combosById[combo.FoodComboId].Price * combo.Quantity);
        var seatTotal = seats.Sum(seat => SeatPricing.GetPrice(showtime.TicketPrice, seat.SeatType));

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var booking = new Booking
            {
                UserId = userId,
                ShowtimeId = showtime.Id,
                CustomerName = model.CustomerName,
                CustomerPhone = model.CustomerPhone,
                CustomerEmail = model.CustomerEmail,
                BookedAt = DateTime.Now,
                TotalAmount = seatTotal + comboTotal,
                Status = "Đã đặt"
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            foreach (var seat in seats)
            {
                seat.IsBooked = true;

                _context.BookingSeats.Add(new BookingSeat
                {
                    BookingId = booking.Id,
                    SeatId = seat.Id,
                    UnitPrice = SeatPricing.GetPrice(showtime.TicketPrice, seat.SeatType)
                });
            }

            foreach (var selectedCombo in selectedCombos)
            {
                var combo = combosById[selectedCombo.FoodComboId];
                _context.BookingCombos.Add(new BookingCombo
                {
                    BookingId = booking.Id,
                    FoodComboId = combo.Id,
                    ComboName = combo.Name,
                    Quantity = selectedCombo.Quantity,
                    UnitPrice = combo.Price
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            HttpContext.Session.Remove(BookingDraftSession.ShowtimeIdKey);

            return RedirectToAction(nameof(Details), new { id = booking.Id });
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
            ModelState.AddModelError(string.Empty,
                "Không thể lưu đơn đặt vé. Vui lòng thử lại.");
            await PopulateBookingFormAsync(model);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var booking = await GetBookingWithDetailsAsync(id.Value);

        return booking == null ? NotFound() : View(booking);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("Id,CustomerName,CustomerPhone,CustomerEmail")] Booking input)
    {
        if (id != input.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(input);
        }

        var booking = await GetAccessibleBookingsQuery()
            .FirstOrDefaultAsync(booking => booking.Id == id);

        if (booking == null)
        {
            return NotFound();
        }

        booking.CustomerName = input.CustomerName;
        booking.CustomerPhone = input.CustomerPhone;
        booking.CustomerEmail = input.CustomerEmail;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var booking = await GetBookingWithDetailsAsync(id.Value);

        return booking == null ? NotFound() : View(booking);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var booking = await GetAccessibleBookingsQuery()
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            return NotFound();
        }

        foreach (var bookingSeat in booking.BookingSeats)
        {
            if (bookingSeat.Seat != null)
            {
                bookingSeat.Seat.IsBooked = false;
            }
        }

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateBookingFormAsync(BookingCreateViewModel model)
    {
        model.ComboSelections ??= new List<BookingComboSelectionViewModel>();
        model.AvailableCombos = await _context.FoodCombos
            .Where(combo => combo.IsAvailable)
            .OrderBy(combo => combo.Price)
            .ToListAsync();

        foreach (var combo in model.AvailableCombos)
        {
            if (model.ComboSelections.All(selection => selection.FoodComboId != combo.Id))
            {
                model.ComboSelections.Add(new BookingComboSelectionViewModel { FoodComboId = combo.Id });
            }
        }

        var showtimes = await _context.Showtimes
            .Include(s => s.Movie)
            .Include(s => s.Cinema)
            .OrderBy(s => s.StartTime)
            .ToListAsync();

        var showtimeOptions = showtimes.Select(s => new
        {
            s.Id,
            DisplayName = $"{s.Movie?.Title} - {s.Cinema?.Name} - {s.StartTime:dd/MM/yyyy HH:mm}"
        });

        ViewData["ShowtimeId"] = new SelectList(
            showtimeOptions,
            "Id",
            "DisplayName",
            model.ShowtimeId);

        if (model.ShowtimeId == null)
        {
            return;
        }

        var selectedShowtime = showtimes.FirstOrDefault(s => s.Id == model.ShowtimeId);

        if (selectedShowtime == null)
        {
            return;
        }

        model.ShowtimeDescription = $"{selectedShowtime.Movie?.Title} - " +
            $"{selectedShowtime.Cinema?.Name} - " +
            selectedShowtime.StartTime.ToString("dd/MM/yyyy HH:mm");
        model.TicketPrice = selectedShowtime.TicketPrice;
        model.AvailableSeats = await _context.Seats
            .Where(s => s.ShowtimeId == selectedShowtime.Id && !s.IsBooked)
            .OrderBy(s => s.SeatCode)
            .ToListAsync();
    }

    private async Task<Booking?> GetBookingWithDetailsAsync(int id)
    {
        return await GetAccessibleBookingsQuery()
            .Include(b => b.Showtime!)
                .ThenInclude(s => s.Movie!)
                    .ThenInclude(movie => movie.Genre)
            .Include(b => b.Showtime!)
                .ThenInclude(s => s.Cinema)
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
            .Include(b => b.BookingCombos)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    private IQueryable<Booking> GetAccessibleBookingsQuery()
    {
        if (User.IsInRole(IdentitySeedData.AdminRole))
        {
            return _context.Bookings;
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return string.IsNullOrWhiteSpace(userId)
            ? _context.Bookings.Where(_ => false)
            : _context.Bookings.Where(booking => booking.UserId == userId);
    }
}
