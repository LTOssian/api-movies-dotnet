using Movies.Core.Entities;

namespace Movies.Application.MovieUseCases.Services;

public interface IMovieService
{
    Task<bool> CreateAsync(Movie movie);
    Task<Movie?> GetByIdAsync(Guid id);
    Task<Movie?> GetBySlugAsync(string slug);
    Task<IEnumerable<Movie>> GetAllAsync();
    Task<Movie?> UpdateAsync(Movie movie);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsById(Guid id);
}
