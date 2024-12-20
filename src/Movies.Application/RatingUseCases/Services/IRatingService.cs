using Movies.Core.Entities;

namespace Movies.Application.RatingUseCases.Services;

public interface IRatingService
{
    Task<bool> RateMovieAsync(
        int rating,
        Guid movieId,
        Guid userId,
        CancellationToken token = default
    );
    Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default);
    Task<IEnumerable<RatedMovie>> GetRatedMovies(Guid userId, CancellationToken token = default);
}
