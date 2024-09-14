using Movies.Application.MovieUseCases;
using Movies.Application.RatingUseCases.Validators;

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
}
