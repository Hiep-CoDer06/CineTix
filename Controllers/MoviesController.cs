using CineTix.Data;
using CineTix.Models;
using CineTix.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace CineTix.Controllers;

public class MoviesController : Controller
{
    private const long MaxPosterFileSize = 5 * 1024 * 1024;
    private static readonly HashSet<string> AllowedPosterExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    private readonly CineTixDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public MoviesController(CineTixDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index(string? search, int? genreId, string? status, int page = 1)
    {
        const int pageSize = 12;

        var query = _context.Movies
            .Include(movie => movie.Genre)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(movie => movie.Title.Contains(search));
        }

        if (genreId.HasValue)
        {
            query = query.Where(movie => movie.GenreId == genreId.Value);
        }

        if (status == "now-showing")
        {
            query = query.Where(movie => movie.IsNowShowing);
        }
        else if (status == "coming-soon")
        {
            query = query.Where(movie => !movie.IsNowShowing && !movie.IsClassic);
        }
        else if (status == "classic")
        {
            query = query.Where(movie => movie.IsClassic);
        }

        var totalCount = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        var movies = await query
            .OrderByDescending(movie => movie.IsFeatured)
            .ThenByDescending(movie => movie.IsHot)
            .ThenByDescending(movie => movie.IsNowShowing)
            .ThenByDescending(movie => movie.IsClassic)
            .ThenBy(movie => movie.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var genres = await _context.Genres
            .AsNoTracking()
            .OrderBy(genre => genre.Name)
            .ToListAsync();

        return View(new MovieIndexViewModel
        {
            Movies = movies,
            Genres = genres,
            Search = search,
            GenreId = genreId,
            Status = status,
            CurrentPage = page,
            TotalPages = totalPages,
            TotalCount = totalCount
        });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movies
            .Include(movie => movie.Genre)
            .FirstOrDefaultAsync(movie => movie.Id == id);

        if (movie == null)
        {
            return NotFound();
        }

        var showtimes = await _context.Showtimes
            .Include(showtime => showtime.Cinema)
            .Where(showtime => showtime.MovieId == movie.Id)
            .OrderBy(showtime => showtime.StartTime)
            .ToListAsync();

        return View(new MovieDetailsViewModel
        {
            Movie = movie,
            Showtimes = showtimes
        });
    }

    [Authorize(Roles = IdentitySeedData.AdminRole)]
    [HttpGet]
    public IActionResult Create()
    {
        LoadGenres();
        return View();
    }

    [Authorize(Roles = IdentitySeedData.AdminRole)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Movie movie, [FromForm] IFormFile? posterFile)
    {
        if (posterFile is null && Request.HasFormContentType)
        {
            posterFile = Request.Form.Files.GetFile("posterFile")
                         ?? Request.Form.Files.FirstOrDefault();
        }
        AddPosterValidationError(posterFile);

        if (!ModelState.IsValid)
        {
            LoadGenres(movie.GenreId);
            return View(movie);
        }

        if (posterFile is not null)
        {
            movie.PosterUrl = await SavePosterAsync(posterFile);
        }

        try
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            DeletePosterFile(movie.PosterUrl);
            ModelState.AddModelError(string.Empty, "Không thể lưu phim. Vui lòng thử lại.");
            LoadGenres(movie.GenreId);
            return View(movie);
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = IdentitySeedData.AdminRole)]
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movies.FindAsync(id);

        if (movie == null)
        {
            return NotFound();
        }
        LoadGenres(movie.GenreId);
        return View(movie);
    }

    [Authorize(Roles = IdentitySeedData.AdminRole)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Movie input, [FromForm] IFormFile? posterFile)
    {
        if (id != input.Id)
        {
            return NotFound();
        }

        var movie = await _context.Movies.FindAsync(id);
        if (movie == null)
        {
            return NotFound();
        }

        if (posterFile is null && Request.HasFormContentType)
        {
            posterFile = Request.Form.Files.GetFile("posterFile")
                         ?? Request.Form.Files.FirstOrDefault();
        }
        AddPosterValidationError(posterFile);

        if (!ModelState.IsValid)
        {
            input.PosterUrl = movie.PosterUrl;
            LoadGenres(input.GenreId);
            return View(input);
        }

        var oldPosterUrl = movie.PosterUrl;
        string? newPosterUrl = null;

        if (posterFile is not null)
        {
            newPosterUrl = await SavePosterAsync(posterFile);
        }

        movie.Title = input.Title;
        movie.Description = input.Description;
        movie.GenreId = input.GenreId;
        movie.DurationMinutes = input.DurationMinutes;
        movie.ReleaseDate = input.ReleaseDate;
        movie.IsNowShowing = input.IsNowShowing;
        movie.IsHot = input.IsHot;
        movie.IsClassic = input.IsClassic;
        movie.IsFeatured = input.IsFeatured;

        if (newPosterUrl is not null)
        {
            movie.PosterUrl = newPosterUrl;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            DeletePosterFile(newPosterUrl);
            input.PosterUrl = oldPosterUrl;
            ModelState.AddModelError(string.Empty, "Không thể cập nhật phim. Vui lòng thử lại.");
            LoadGenres(input.GenreId);
            return View(input);
        }

        if (newPosterUrl is not null)
        {
            DeletePosterFile(oldPosterUrl);
        }

        return RedirectToAction(nameof(Index));
    }
    [Authorize(Roles = IdentitySeedData.AdminRole)]
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movies.FindAsync(id);

        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    [Authorize(Roles = IdentitySeedData.AdminRole)]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var movie = await _context.Movies.FindAsync(id);

        if (movie != null)
        {
            var posterUrl = movie.PosterUrl;
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            DeletePosterFile(posterUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    private void LoadGenres(int? selectedGenreId = null)
    {
        ViewData["GenreId"] = new SelectList(
            _context.Genres.OrderBy(g => g.Name),
            "Id",
            "Name",
            selectedGenreId);
    }

    private void AddPosterValidationError(IFormFile? posterFile)
    {
        if (posterFile is null)
        {
            return;
        }

        if (posterFile.Length == 0)
        {
            ModelState.AddModelError("posterFile", "Tệp ảnh đang rỗng.");
            return;
        }

        if (posterFile.Length > MaxPosterFileSize)
        {
            ModelState.AddModelError("posterFile", "Ảnh poster không được lớn hơn 5 MB.");
            return;
        }

        var extension = Path.GetExtension(posterFile.FileName);
        if (!AllowedPosterExtensions.Contains(extension))
        {
            ModelState.AddModelError("posterFile", "Chỉ chấp nhận ảnh JPG, JPEG, PNG hoặc WEBP.");
        }
    }

    private async Task<string> SavePosterAsync(IFormFile posterFile)
    {
        var extension = Path.GetExtension(posterFile.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
        var posterFolderPath = Path.Combine(webRootPath, "uploads", "posters");

        Directory.CreateDirectory(posterFolderPath);

        var filePath = Path.Combine(posterFolderPath, fileName);
        await using var stream = new FileStream(filePath, FileMode.CreateNew);
        await posterFile.CopyToAsync(stream);

        return $"/uploads/posters/{fileName}";
    }

    private void DeletePosterFile(string? posterUrl)
    {
        const string posterPathPrefix = "/uploads/posters/";

        if (string.IsNullOrWhiteSpace(posterUrl) ||
            !posterUrl.StartsWith(posterPathPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var fileName = Path.GetFileName(posterUrl);
        var webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
        var filePath = Path.Combine(webRootPath, "uploads", "posters", fileName);

        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
    }
}
