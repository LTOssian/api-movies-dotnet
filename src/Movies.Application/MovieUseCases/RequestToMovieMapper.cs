using Movies.Core.Entities;

namespace Movies.Application.MovieUseCases;

public static class RequestToMovieMapper
{
    public static Movie ToMovie(this CreateMovieRequest self)
    {
        var movie = new Movie
        {
            Id = Guid.NewGuid(),
            Title = self.Title,
            YearOfRelease = self.YearOfRelease,
            Genres = self.Genres.ToList()
        };

        return movie;
    }

    public static MovieResponse ToResponse(this CreateMovieRequest self, Movie movie)
    {
        var response = new MovieResponse
        {
            Id = movie.Id,
            Title = movie.Title,
            YearOfRelease = movie.YearOfRelease,
            Genres = movie.Genres.AsEnumerable()
        };

        return response;
    }
}
