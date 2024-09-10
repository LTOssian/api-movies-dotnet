using Movies.Application.MovieUseCases;
using Movies.Infrastructure.Database;
using Movies.Infrastructure.Repositories.InMemory;

namespace Movies.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IMovieRepository, MovieRepository>();
        return services;
    }

    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddControllers();
        services.AddProblemDetails();

        return services;
    }

    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        string connectionString
    )
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(
            connectionString
        ));
        services.AddSingleton<DatabaseInitializer>();

        return services;
    }
}
