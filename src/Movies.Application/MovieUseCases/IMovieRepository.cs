using Movies.Core.Entities;

namespace Movies.Application.MovieUseCases;

public interface IMovieRepository
{
    Task<bool> CreateAsync(Movie movie);
    Task<Movie?> GetByIdAsync(Guid id);
    Task<Movie?> GetBySlugAsync(string slug);
    Task<IEnumerable<Movie>> GetAllAsync();
    Task<bool> UpdateAsync(Movie movie);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsById(Guid id);
}
