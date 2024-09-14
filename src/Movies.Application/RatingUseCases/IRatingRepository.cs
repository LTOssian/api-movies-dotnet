namespace Movies.Application.RatingUseCases;

public interface IRatingRepository
{
    Task<float?> GetRatingAsync(Guid movieId, CancellationToken token);
    Task<(float? Rating, int? UserRating)> GetUserRatingAsync(Guid movieId, Guid userId, CancellationToken token);
}
