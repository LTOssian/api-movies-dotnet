using Movies.Application.MovieUseCases;
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

    public static MoviesResponse ToResponse(
        this IEnumerable<Movie> self,
        int page,
        int pageSize,
        int moviesCount
    )
    {
        var response = new MoviesResponse
        {
            Items = self.Select(ToResponse),
            Count = moviesCount,
            Page = page,
            PageSize = pageSize
        };

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

    public static GetAllMoviesOptions ToOptions(this GetAllMoviesRequest request) =>
        new GetAllMoviesOptions
        {
            Title = request.Title,
            YearOfRelease = request.Year,
            SortField = request.SortBy?.TrimStart('+', '-', ' '),
            SortOrder = request.SortBy is null
                ? SortOrder.Unsorted
                : request.SortBy.StartsWith('-')
                    ? SortOrder.Descending
                    : SortOrder.Ascending,
            Page = request.Page,
            PageSize = request.PageSize
        };

    public static GetAllMoviesOptions WithUserId(this GetAllMoviesOptions self, Guid? userId)
    {
        self.UserId = userId;
        return self;
    }
}
