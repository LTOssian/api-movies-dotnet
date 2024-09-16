using Movies.Core.Entities;

namespace Movies.Application.MovieUseCases.Services;

public interface IMovieService
{
    Task<bool> CreateAsync(Movie movie, CancellationToken token = default);
    Task<Movie?> GetByIdAsync(Guid id, Guid? userId, CancellationToken token = default);
    Task<Movie?> GetBySlugAsync(string slug, Guid? userId, CancellationToken token = default);
    Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default);
    Task<Movie?> UpdateAsync(Movie movie, Guid? userId, CancellationToken token = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken token = default);
    Task<bool> ExistsById(Guid id, CancellationToken token = default);

    Task<int> GetCountAsync(string? title, int? yearofrelease, CancellationToken token = default);
}
