using Movies.Application.CreateMovieUseCase;
using Movies.Core.Entities;

namespace Movies.Application.MovieUseCases;

public static class RequestToMovieMapper
{
    public static Movie ToMovie(this CreateMovieRequest self) => new Movie
    {
        Id = Guid.NewGuid(),
        Title = self.Title,
        YearOfRelease = self.YearOfRelease,
        Genres = self.Genres.ToList()
    };

    public static CreateMovieResponse ToResponse(this CreateMovieRequest self, Movie movie) => new CreateMovieResponse
    {
        Id = movie.Id,
        Title = movie.Title,
        YearOfRelease = movie.YearOfRelease,
        Genres = movie.Genres.AsEnumerable()
    };
}
