using Movies.Application.MovieUseCases;
using Movies.Application.RatingUseCases.Validators;
using Movies.Core.Entities;

namespace Movies.Application.RatingUseCases.Services;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly RatingValidator _validator;

    public RatingService(
        IRatingRepository ratingRepository,
        IMovieRepository movieRepository,
        RatingValidator validations
    )
    {
        _ratingRepository = ratingRepository;
        _movieRepository = movieRepository;
        _validator = validations;
    }

    public async Task<bool> RateMovieAsync(
        int rating,
        Guid movieId,
        Guid userId,
        CancellationToken token
    )
    {
        await _validator.ValidateAsync(rating, token);

        var movieExists = await _movieRepository.ExistsById(movieId, token);
        if (!movieExists)
        {
            return false;
        }

        return await _ratingRepository.RateMovieAsync(movieId, rating, userId, token);
    }

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token)
    {
        var ratingExists = await _ratingRepository.ExistsByMovieAndUserIds(movieId, userId, token);
        if (!ratingExists)
        {
            return false;
        }

        return await _ratingRepository.DeleteRatingAsync(movieId, userId, token);
    }

    public async Task<IEnumerable<RatedMovie>> GetRatedMovies(Guid userId, CancellationToken token = default)
    {
        return await _ratingRepository.GetRatedMoviesAsync(userId, token);
    }
}
