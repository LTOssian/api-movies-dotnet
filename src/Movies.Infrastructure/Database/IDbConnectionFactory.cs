using System;
using System.Data;
using Npgsql;

namespace Movies.Infrastructure.Database;

public interface IDbConnectionFactory
{
    Task<NpgsqlConnection> CreateConnectionAsync();
}

public class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionSttring;

    public NpgsqlConnectionFactory(string connectionString)
    {
        _connectionSttring = connectionString;
    }

    public async Task<NpgsqlConnection> CreateConnectionAsync()
    {
        var connection = new NpgsqlConnection(_connectionSttring);
        await connection.OpenAsync();
        
        return connection;
    }
}