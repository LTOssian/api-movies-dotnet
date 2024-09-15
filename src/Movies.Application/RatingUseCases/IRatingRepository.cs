namespace Movies.Application.RatingUseCases;

public interface IRatingRepository
{
    Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken token);
    Task<float?> GetRatingAsync(Guid movieId, CancellationToken token);
    Task<(float? Rating, int? UserRating)> GetUserRatingAsync(Guid movieId, Guid userId, CancellationToken token);
    Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token);
    Task<bool> ExistsByMovieAndUserIds(Guid movieId, Guid userId, CancellationToken token);
}
