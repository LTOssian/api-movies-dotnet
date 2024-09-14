namespace Movies.Application.RatingUseCases.Services;

public interface IRatingService
{
    Task<bool> RateMovieAsync(int rating, Guid movieId, Guid userId, CancellationToken token);
}