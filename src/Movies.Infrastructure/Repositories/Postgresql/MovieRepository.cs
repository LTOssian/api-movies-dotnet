using System;
using System.Data;
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
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = await connection.BeginTransactionAsync();

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

        await transaction.CommitAsync();

        return movieInsertResult > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = await connection.BeginTransactionAsync();

        await connection.ExecuteAsync(
            new CommandDefinition(
                """
                    DELETE FROM genres 
                    WHERE movie_id = @id
                """,
                new { id },
                transaction,
                cancellationToken: token
            )
        );

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

        await transaction.CommitAsync();
        return deletedMovieResult > 0;
    }

    public async Task<bool> ExistsById(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
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

    public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var movieSelectResult = await connection.QueryAsync(
            new CommandDefinition(
                """
                    SELECT m.*, string_agg(g.name, ',') as genres
                    FROM movies AS m 
                    LEFT JOIN genres AS g on m.id = g.movie_id
                    GROUP BY id
                """,
                cancellationToken: token
            )
        );

        return movieSelectResult.Select(x => new Movie
        {
            Id = x.id,
            Title = x.title,
            YearOfRelease = x.year_of_release,
            Genres = Enumerable.ToList(x.genres.Split(','))
        });
    }

    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition(
                """
                    SELECT *, movies.year_of_release AS YearOfRelease
                    FROM movies 
                    WHERE id = @id
                """,
                new { id },
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

    public async Task<Movie?> GetBySlugAsync(string slug, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition(
                """
                    SELECT *, movies.year_of_release AS YearOfRelease
                    FROM movies 
                    WHERE slug = @slug
                """,
                new { slug },
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

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = await connection.BeginTransactionAsync();

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

        await transaction.CommitAsync();
        return updatedMovieResult > 0;
    }
}
