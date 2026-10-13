
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CineTix.Models;
using CineTix.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize(Roles = IdentitySeedData.AdminRole)]
public class SeatsController : Controller
{
    private readonly CineTixDbContext _context;

    public SeatsController(CineTixDbContext context)
    {
        _context = context;
    }

    // GET: SEATS
    public async Task<IActionResult> Index()
    {
        var seats = await _context.Seats
            .Include(s => s.Showtime!)
                .ThenInclude(showtime => showtime.Movie)
            .Include(s => s.Showtime!)
                .ThenInclude(showtime => showtime.Cinema)
            .OrderBy(s => s.Showtime!.StartTime)
            .ThenBy(s => s.SeatCode)
            .ToListAsync();

        return View(seats);
    }

    // GET: SEATS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var seat = await _context.Seats
    .Include(s => s.Showtime!)
        .ThenInclude(showtime => showtime.Movie)
    .Include(s => s.Showtime!)
        .ThenInclude(showtime => showtime.Cinema)
    .FirstOrDefaultAsync(s => s.Id == id);
        if (seat == null)
        {
            return NotFound();
        }
        
        return View(seat);
    }

    // GET: SEATS/Create
    public IActionResult Create()
    {
        LoadShowtimes();
        return View();
    }

    // POST: SEATS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,ShowtimeId,SeatCode,SeatType,IsBooked")] Seat seat)
    {
        if (ModelState.IsValid)
        {
            _context.Add(seat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        LoadShowtimes(seat.ShowtimeId);
        return View(seat);
    }

    // GET: SEATS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var seat = await _context.Seats.FindAsync(id);
        if (seat == null)
        {
            return NotFound();
        }
        LoadShowtimes(seat.ShowtimeId);
        return View(seat);
    }

    // POST: SEATS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,ShowtimeId,SeatCode,SeatType,IsBooked")] Seat seat)
    {
        if (id != seat.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(seat);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SeatExists(seat.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        LoadShowtimes(seat.ShowtimeId);
        return View(seat);
    }

    // GET: SEATS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var seat = await _context.Seats
    .Include(s => s.Showtime!)
        .ThenInclude(showtime => showtime.Movie)
    .Include(s => s.Showtime!)
        .ThenInclude(showtime => showtime.Cinema)
    .FirstOrDefaultAsync(s => s.Id == id);
           
        if (seat == null)
        {
            return NotFound();
        }

        return View(seat);
    }

    // POST: SEATS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var seat = await _context.Seats.FindAsync(id);
        if (seat != null)
        {
            _context.Seats.Remove(seat);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private void LoadShowtimes(int? selectedShowtimeId = null)
    {
        var showtimes = _context.Showtimes
            .Include(s => s.Movie)
            .Include(s => s.Cinema)
            .OrderBy(s => s.StartTime)
            .ToList()
            .Select(s => new
            {
                s.Id,
                DisplayName =
                    $"{s.Movie?.Title} - {s.Cinema?.Name} - " +
                    $"{s.StartTime:dd/MM/yyyy HH:mm}"
            });

        ViewData["ShowtimeId"] = new SelectList(
            showtimes,
            "Id",
            "DisplayName",
            selectedShowtimeId);
    }

    private bool SeatExists(int? id)
    {
        return _context.Seats.Any(e => e.Id == id);
    }

}
