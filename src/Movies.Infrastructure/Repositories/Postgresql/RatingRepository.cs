using Dapper;
using Movies.Application.RatingUseCases;
using Movies.Infrastructure.Database;

namespace Movies.Infrastructure.Repositories.Postgresql;

public class RatingRepository : IRatingRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public RatingRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken token)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        var rating = await connection.QuerySingleOrDefaultAsync<float?>(
            new CommandDefinition(
                """
                    SELECT round(avg(r.rating), 1)
                    FROM ratings AS r
                    WHERE movie_id = @movieId
                """,
                new { movieId },
                cancellationToken: token
            )
        );

        return rating;
    }

    public async Task<(float? Rating, int? UserRating)> GetUserRatingAsync(
        Guid movieId,
        Guid userId,
        CancellationToken token
    )
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        var ratings = await connection.QuerySingleOrDefaultAsync<(float?, int?)>(
            new CommandDefinition(
                """
                    SELECT round(avg(r.rating), 1), MAX(
                            CASE
                                WHEN r.user_id = @userId THEN r.rating
                                ELSE NULL
                            END
                        )
                    FROM ratings AS r
                    WHERE movie_id = @movieId
                """,
                new { movieId, userId },
                cancellationToken: token
            )
        );

        return ratings;
    }

    public async Task<bool> RateMovieAsync(
        Guid movieId,
        int rating,
        Guid userId,
        CancellationToken token
    )
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = await connection.BeginTransactionAsync();

        var createdRating = await connection.ExecuteAsync(
            new CommandDefinition(
                """
                    INSERT INTO ratings (user_id, movie_id, rating)
                    VALUES (@userId, @movieId, @rating)
                    ON CONFLICT (@userId, @movieId,)DO UPDATE
                        SET rating = @rating
                """,
                new
                {
                    userId,
                    movieId,
                    rating
                },
                transaction,
                cancellationToken: token
            )
        );

        await transaction.CommitAsync();
        return createdRating > 0;
    }
}
