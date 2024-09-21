using Dapper;
using Movies.Application.RatingUseCases;
using Movies.Core.Entities;
using Movies.Infrastructure.Database;

namespace Movies.Infrastructure.Repositories.Postgresql;

public class RatingRepository : IRatingRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public RatingRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token)
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(token);

        var deletedRatingResult = await connection.ExecuteAsync(
            new CommandDefinition(
                """
                    DELETE FROM ratings
                    WHERE movie_id = @movieId AND user_id = @userId
                """,
                new { movieId, userId },
                transaction,
                cancellationToken: token
            )
        );

        await transaction.CommitAsync(token);
        return deletedRatingResult > 0;
    }

    public async Task<bool> ExistsByMovieAndUserIds(
        Guid movieId,
        Guid userId,
        CancellationToken token
    )
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                """
                    SELECT count(1)
                    FROM ratings
                    WHERE movie_id = @movieId AND user_id = @userId
                """,
                new { movieId, userId },
                cancellationToken: token
            )
        );
    }

    public async Task<IEnumerable<RatedMovie>> GetRatedMoviesAsync(
        Guid userId,
        CancellationToken token
    )
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        var ratedMoviesResult = await connection.QueryAsync<RatedMovie>(
            new CommandDefinition(
                """
                    SELECT r.movie_id AS movieId, m.slug AS slug, r.rating AS rating
                    FROM movies AS m
                    JOIN ratings AS r ON m.id = r.movie_id
                    AND r.user_id = @userId
                """,
                new { userId },
                cancellationToken: token
            )
        );

        return ratedMoviesResult;
    }

    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken token)
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

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
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        var ratings = await connection.QuerySingleOrDefaultAsync<(float?, int?)>(
            new CommandDefinition(
                """
                    SELECT round(avg(r.rating), 1), MAX(
                            CASE
                                WHEN r.user_id = @userId THEN r.rating
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
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(token);

        var createdRating = await connection.ExecuteAsync(
            new CommandDefinition(
                """
                    INSERT INTO ratings (user_id, movie_id, rating)
                    VALUES (@userId, @movieId, @rating)
                    ON CONFLICT (user_id, movie_id) 
                    DO UPDATE SET rating = EXCLUDED.rating
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

        await transaction.CommitAsync(token);
        return createdRating > 0;
    }
}
