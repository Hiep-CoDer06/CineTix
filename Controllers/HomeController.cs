using System.Diagnostics;
using CineTix.Data;
using CineTix.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineTix.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CineTixDbContext _context;

        public HomeController(ILogger<HomeController> logger, CineTixDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies
                .Include(movie => movie.Genre)
                .OrderByDescending(movie => movie.IsFeatured)
                .ThenByDescending(movie => movie.IsHot)
                .ThenByDescending(movie => movie.IsNowShowing)
                .ThenByDescending(movie => movie.ReleaseDate)
                .Take(8)
                .ToListAsync();

            return View(movies);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
