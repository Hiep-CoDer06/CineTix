using CineTix.Data;
using CineTix.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineTix.Controllers;

[Authorize(Roles = IdentitySeedData.AdminRole)]
public class AdminController : Controller
{
    private readonly CineTixDbContext _context;

    public AdminController(CineTixDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var todayBookings = _context.Bookings
            .Where(booking => booking.BookedAt >= today && booking.BookedAt < tomorrow);

        var dashboard = new AdminDashboardViewModel
        {
            TodayRevenue = await todayBookings
                .Select(booking => (decimal?)booking.TotalAmount)
                .SumAsync() ?? 0m,
            TodayBookings = await todayBookings.CountAsync(),
            TodayShowtimes = await _context.Showtimes
                .CountAsync(showtime => showtime.StartTime >= today && showtime.StartTime < tomorrow),
            NowShowingMovies = await _context.Movies.CountAsync(movie => movie.IsNowShowing),
            RecentMovies = await _context.Movies
                .Include(movie => movie.Genre)
                .OrderByDescending(movie => movie.IsNowShowing)
                .ThenBy(movie => movie.Title)
                .Take(6)
                .ToListAsync()
        };

        return View(dashboard);
    }
}
