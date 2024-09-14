using Movies.Core.Entities;

namespace Movies.Application.MovieUseCases;

public interface IMovieRepository
{
    Task<bool> CreateAsync(Movie movie, CancellationToken token = default);
    Task<Movie?> GetByIdAsync(Guid id, Guid? userId, CancellationToken token = default);
    Task<Movie?> GetBySlugAsync(string slug, Guid? userId, CancellationToken token = default);
    Task<IEnumerable<Movie>> GetAllAsync(Guid? userId, CancellationToken token = default);
    Task<bool> UpdateAsync(Movie movie, CancellationToken token = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken token = default);
    Task<bool> ExistsById(Guid id, CancellationToken token = default);
}
