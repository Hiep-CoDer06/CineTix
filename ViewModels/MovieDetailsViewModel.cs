using CineTix.Models;

namespace CineTix.ViewModels;

public class MovieDetailsViewModel
{
    public Movie Movie { get; set; } = null!;

    public List<Showtime> Showtimes { get; set; } = new();
}
