namespace Movies.Application.MovieUseCases;

public class MoviesResponse
{
    public required IEnumerable<MovieResponse> Items { get; init; }
}
