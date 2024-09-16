using FluentValidation;
using Movies.Application.MovieUseCases.Validators;
using Movies.Application.RatingUseCases;
using Movies.Core.Entities;

namespace Movies.Application.MovieUseCases.Services;

public class MovieService : IMovieService
{
    private readonly MovieValidator _movieValidator;
    private readonly GetAllMoviesOptionsValidator _optionsValidator;
    private readonly IMovieRepository _movieRepository;
    private readonly IRatingRepository _ratingRepository;

    public MovieService(IMovieRepository movieRepository, MovieValidator movieValidator, IRatingRepository ratingRepository, GetAllMoviesOptionsValidator optionsValidator)
    {
        _movieValidator = movieValidator;
        _optionsValidator = optionsValidator;
        _movieRepository = movieRepository;
        _ratingRepository = ratingRepository;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, token);

        return await _movieRepository.CreateAsync(movie, token);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
    {
        return await _movieRepository.DeleteAsync(id, token);
    }

    public async Task<bool> ExistsById(Guid id, CancellationToken token = default)
    {
        return await _movieRepository.ExistsById(id, token);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(
        GetAllMoviesOptions options,
        CancellationToken token = default
    )
    {
        await _optionsValidator.ValidateAsync(options);
        return await _movieRepository.GetAllAsync(options, token);
    }

    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId, CancellationToken token = default)
    {
        return await _movieRepository.GetByIdAsync(id, userId, token);
    }

    public async Task<Movie?> GetBySlugAsync(
        string slug,
        Guid? userId,
        CancellationToken token = default
    )
    {
        return await _movieRepository.GetBySlugAsync(slug, userId, token);
    }

    public async Task<Movie?> UpdateAsync(
        Movie movie,
        Guid? userId,
        CancellationToken token = default
    )
    {
        await _movieValidator.ValidateAndThrowAsync(movie, token);

        var movieExists = await _movieRepository.ExistsById(movie.Id, token);
        if (!movieExists)
            return null;

        await _movieRepository.UpdateAsync(movie, token);

        if (!userId.HasValue)
        {
            var rating = await _ratingRepository.GetRatingAsync(movie.Id, token);
            movie.Rating = rating;
            return movie;
        }

        var ratings = await _ratingRepository.GetUserRatingAsync(movie.Id, userId.Value, token);
        movie.Rating = ratings.Rating;
        movie.UserRating = ratings.UserRating;

        return movie;
    }
}
