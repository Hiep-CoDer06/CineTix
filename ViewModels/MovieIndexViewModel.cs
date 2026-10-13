using CineTix.Models;

namespace CineTix.ViewModels;

public sealed class MovieIndexViewModel
{
    public IReadOnlyList<Movie> Movies { get; init; } = Array.Empty<Movie>();
    public IReadOnlyList<Genre> Genres { get; init; } = Array.Empty<Genre>();
    public string? Search { get; init; }
    public int? GenreId { get; init; }
    public string? Status { get; init; }
    public int CurrentPage { get; init; }
    public int TotalPages { get; init; }
    public int TotalCount { get; init; }

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}
