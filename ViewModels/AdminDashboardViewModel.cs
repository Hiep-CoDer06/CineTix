using CineTix.Models;

namespace CineTix.ViewModels;

public class AdminDashboardViewModel
{
    public decimal TodayRevenue { get; set; }
    public int TodayBookings { get; set; }
    public int TodayShowtimes { get; set; }
    public int NowShowingMovies { get; set; }
    public List<Movie> RecentMovies { get; set; } = new();
}
