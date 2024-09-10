using System.Diagnostics.CodeAnalysis;
using Dapper;

namespace Movies.Infrastructure.Database;

public class DatabaseInitializer
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DatabaseInitializer(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        await connection.ExecuteAsync(
            """
                create table if not exists movies (
                id UUID primary key,
                slug TEXT not null,
                title TEXT not null,
                year_of_release integer not null);
            """
        );

        await connection.ExecuteAsync(
            """
                create unique index concurrently if not exists movies_slug_idx
                on movies
                using btree(slug);
            """
        );

        await connection.ExecuteAsync(
            """
                create table if not exists genres (
                movie_id UUID references movies (id),
                name TEXT not null);
            """
        );
    }
}
