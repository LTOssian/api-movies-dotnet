using Movies.Contracts.Requests;
using Movies.Contracts.Responses;
using Movies.Core.Entities;

namespace Movies.Api.Mapping;

public static class RequestToMovieMapper
{
    public static Movie ToMovie(this CreateMovieRequest self) =>
        new Movie
        {
            Id = Guid.NewGuid(),
            Title = self.Title,
            YearOfRelease = self.YearOfRelease,
            Genres = self.Genres.ToList()
        };

    public static CreateMovieResponse ToResponse(this CreateMovieRequest self, Movie movie) =>
        new CreateMovieResponse
        {
            Id = movie.Id,
            Title = movie.Title,
            Slug = movie.Slug,
            Rating = movie.Rating,
            UserRating = movie.UserRating,
            YearOfRelease = movie.YearOfRelease,
            Genres = movie.Genres.AsEnumerable()
        };

    public static MovieResponse ToResponse(this Movie self) =>
        new CreateMovieResponse
        {
            Id = self.Id,
            Title = self.Title,
            Slug = self.Slug,
            Rating = self.Rating,
            UserRating = self.UserRating,
            YearOfRelease = self.YearOfRelease,
            Genres = self.Genres.AsEnumerable()
        };

    public static MoviesResponse ToResponse(this IEnumerable<Movie> self)
    {
        var response = new MoviesResponse { Items = self.Select(ToResponse) };

        return response;
    }

    public static Movie ToMovie(this UpdateMovieRequest self, Guid id) =>
        new Movie
        {
            Id = id,
            Title = self.Title,
            YearOfRelease = self.YearOfRelease,
            Genres = self.Genres.ToList()
        };

    public static RatedMovieResponse ToResponse(this RatedMovie self) =>
        new RatedMovieResponse
        {
            MovieId = self.MovieId,
            Slug = self.Slug,
            Rating = self.Rating
        };
}
