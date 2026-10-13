
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CineTix.Models;
using CineTix.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize(Roles = IdentitySeedData.AdminRole)]
public class ShowtimesController : Controller
{
    private readonly CineTixDbContext _context;

    public ShowtimesController(CineTixDbContext context)
    {
        _context = context;
    }

    // GET: SHOWTIMES
    public async Task<IActionResult> Index()
    {
        var showtimes = await _context.Showtimes
            .Include(s => s.Movie)
            .Include(s => s.Cinema)
            .ToListAsync();

        return View(showtimes);
    }

    // GET: SHOWTIMES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var showtime = await _context.Showtimes
    .Include(s => s.Movie)
    .Include(s => s.Cinema)
    .FirstOrDefaultAsync(s => s.Id == id);
        if (showtime == null)
        {
            return NotFound();
        }

        return View(showtime);
    }

    // GET: SHOWTIMES/Create
    public IActionResult Create()
    {
        LoadSelections();
        return View();
    }

    // POST: SHOWTIMES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,MovieId,CinemaId,StartTime,RoomName,TicketPrice")] Showtime showtime)
    {
        if (ModelState.IsValid)
        {
            _context.Add(showtime);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        LoadSelections(showtime.MovieId, showtime.CinemaId);
        return View(showtime);
    }

    // GET: SHOWTIMES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var showtime = await _context.Showtimes.FindAsync(id);
        if (showtime == null)
        {
            return NotFound();
        }
        LoadSelections(showtime.MovieId, showtime.CinemaId);
        return View(showtime);
    }

    // POST: SHOWTIMES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id, MovieId, CinemaId, StartTime, RoomName, TicketPrice")] Showtime showtime)
    {
        if (id != showtime.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(showtime);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShowtimeExists(showtime.Id))
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
        LoadSelections(showtime.MovieId, showtime.CinemaId);
        return View(showtime);
    }

    // GET: SHOWTIMES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var showtime = await _context.Showtimes
    .Include(s => s.Movie)
    .Include(s => s.Cinema)
    .FirstOrDefaultAsync(s => s.Id == id);
        if (showtime == null)
        {
            return NotFound();
        }

        return View(showtime);
    }

    // POST: SHOWTIMES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var showtime = await _context.Showtimes.FindAsync(id);
        if (showtime != null)
        {
            _context.Showtimes.Remove(showtime);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private void LoadSelections(int? selectedMovieId = null, int? selectedCinemaId = null)
    {
        ViewData["MovieId"] = new SelectList(
            _context.Movies.OrderBy(m => m.Title),
            "Id",
            "Title",
            selectedMovieId);

        ViewData["CinemaId"] = new SelectList(
            _context.Cinemas.OrderBy(c => c.Name),
            "Id",
            "Name",
            selectedCinemaId);
    }

    private bool ShowtimeExists(int? id)
    {
        return _context.Showtimes.Any(e => e.Id == id);
    }
}
