using Dapper;
using Movies.Application.MovieUseCases;
using Movies.Core.Entities;
using Movies.Infrastructure.Database;

namespace Movies.Infrastructure.Repositories.Postgresql;

public class MovieRepository : IMovieRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public MovieRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(token);

        var movieInsertResult = await connection.ExecuteAsync(
            new CommandDefinition(
                """
                    insert into movies (id, slug, title, year_of_release)
                    values(@Id, @Slug, @Title, @YearOfRelease)
                """,
                movie,
                transaction,
                cancellationToken: token
            )
        );

        if (movieInsertResult > 0)
        {
            var genreInsertQuery = """
                    insert into genres (movie_id, name)
                    values (@MovieId, @Name)
                """;

            var genreValues = movie.Genres.Select(genre => new
            {
                MovieId = movie.Id,
                Name = genre
            });

            await connection.ExecuteAsync(
                new CommandDefinition(
                    genreInsertQuery,
                    genreValues,
                    transaction,
                    cancellationToken: token
                )
            );
        }

        await transaction.CommitAsync(token);

        return movieInsertResult > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(token);

        var deletedMovieResult = await connection.ExecuteAsync(
            new CommandDefinition(
                """
                    DELETE FROM movies 
                    WHERE id = @id
                """,
                new { id },
                transaction,
                cancellationToken: token
            )
        );

        await transaction.CommitAsync(token);
        return deletedMovieResult > 0;
    }

    public async Task<bool> ExistsById(Guid id, CancellationToken token = default)
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                """
                    SELECT count(1) 
                    FROM movies 
                    WHERE id = @id
                """,
                new { id },
                cancellationToken: token
            )
        );
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(
        GetAllMoviesOptions options,
        CancellationToken token = default
    )
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        var orderClause = string.Empty;
        if (options.SortField is not null)
        {
            orderClause = $"""
                , m.{options.SortField} 
                ORDER BY m.{options.SortField} {(
                    options.SortOrder == SortOrder.Ascending ? "ASC" : "DESC"
                )}
                """;
        }

        var movieSelectResult = await connection.QueryAsync(
            new CommandDefinition(
                $"""
                    SELECT m.*, string_agg(DISTINCT g.name, ',') as genres, round(avg(r.rating), 1) AS rating, my_r.rating AS user_rating
                    FROM movies AS m 
                    LEFT JOIN genres AS g on m.id = g.movie_id
                    LEFT JOIN ratings AS r ON m.id = r.movie_id
                    LEFT JOIN ratings AS my_r ON m.id = my_r.movie_id 
                        AND my_r.user_id = @userId
                    WHERE (@title IS NULL OR LOWER(m.title) LIKE ('%' || LOWER(@title) || '%'))
                        AND (@yearofrelease IS NULL OR m.year_of_release = @yearofrelease)
                    GROUP BY m.id, user_rating {orderClause}
                    LIMIT @pageSize
                    OFFSET @pageOffset
                """,
                new
                {
                    userId = options.UserId,
                    title = options.Title,
                    yearofrelease = options.YearOfRelease,
                    pageSize = options.PageSize,
                    pageOffset = (options.Page - 1) * options.PageSize
                },
                cancellationToken: token
            )
        );

        var res = movieSelectResult.Select(x => new Movie
        {
            Id = x.id,
            Title = x.title,
            YearOfRelease = x.year_of_release,
            Rating = (float?)x.rating,
            UserRating = (int?)x.user_rating,
            Genres = Enumerable.ToList(x.genres.Split(','))
        });

        return res;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId, CancellationToken token = default)
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition(
                """
                    SELECT m.*, m.year_of_release AS YearOfRelease, round(avg(r.rating), 1) AS rating, my_r.rating AS UserRating
                    FROM movies AS m
                    LEFT JOIN ratings AS r ON m.id = r.movie_id
                    LEFT JOIN ratings AS my_r ON m.id = my_r.movie_id 
                        AND my_r.user_id = @userId
                    WHERE m.id = @id
                    GROUP BY m.id, UserRating
                """,
                new { id, userId },
                cancellationToken: token
            )
        );

        if (movie is null)
        {
            return null;
        }

        var genres = await connection.QueryAsync<string>(
            new CommandDefinition(
                """
                    SELECT name 
                    FROM genres 
                    WHERE movie_id = @id
                """,
                new { id },
                cancellationToken: token
            )
        );

        movie.Genres.AddRange(genres);

        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(
        string slug,
        Guid? userId,
        CancellationToken token = default
    )
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition(
                """
                    SELECT
                        m.*,
                        m.year_of_release AS YearOfRelease,
                        round(avg(r.rating), 1) AS rating,
                        my_r.rating AS UserRating
                    FROM
                        movies AS m
                        LEFT JOIN ratings AS r ON m.id = r.movie_id
                        LEFT JOIN ratings AS my_r ON m.id = my_r.movie_id
                        AND my_r.user_id = @userId
                    WHERE
                        m.slug=@slug
                    GROUP BY
                        m.id,
                        UserRating
                """,
                new { slug, userId },
                cancellationToken: token
            )
        );

        if (movie is null)
        {
            return null;
        }

        var genres = await connection.QueryAsync<string>(
            new CommandDefinition(
                """
                    SELECT name 
                    FROM genres 
                    WHERE movie_id = @id
                """,
                new { id = movie.Id },
                cancellationToken: token
            )
        );

        movie.Genres.AddRange(genres);

        return movie;
    }

    public async Task<int> GetCountAsync(
        string? title,
        int? yearofrelease,
        CancellationToken token = default
    )
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await connection.QuerySingleAsync<int>(
            new CommandDefinition(
                """
                    SELECT count(id) FROM movies
                    WHERE (@title IS NULL OR LOWER(title) LIKE ('%' || LOWER(@title) || '%'))
                        AND (@yearofrelease IS NULL OR year_of_release = @yearofrelease)
                """,
                new { yearofrelease, title },
                cancellationToken: token
            )
        );
    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(token);

        await connection.ExecuteAsync(
            new CommandDefinition(
                """
                    DELETE FROM genres WHERE movie_id = @id
                """,
                new { id = movie.Id },
                transaction,
                cancellationToken: token
            )
        );

        var genreInsertQuery = """
                insert into genres (movie_id, name)
                values (@MovieId, @Name)
            """;
        var genreValues = movie.Genres.Select(genre => new { MovieId = movie.Id, Name = genre });
        await connection.ExecuteAsync(
            new CommandDefinition(
                genreInsertQuery,
                genreValues,
                transaction,
                cancellationToken: token
            )
        );

        var updatedMovieResult = await connection.ExecuteAsync(
            new CommandDefinition(
                """
                    UPDATE movies 
                    SET slug = @Slug, title = @Title, year_of_release = @YearOfRelease
                    WHERE id = @Id
                """,
                movie,
                transaction,
                cancellationToken: token
            )
        );

        await transaction.CommitAsync(token);
        return updatedMovieResult > 0;
    }
}
