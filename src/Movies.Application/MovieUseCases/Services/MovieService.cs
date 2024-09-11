using Movies.Core.Entities;

namespace Movies.Application.MovieUseCases.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public MovieService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public Task<bool> CreateAsync(Movie movie)
    {
        return _movieRepository.CreateAsync(movie);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return _movieRepository.DeleteAsync(id);
    }

    public Task<bool> ExistsById(Guid id)
    {
        return _movieRepository.ExistsById(id);
    }

    public Task<IEnumerable<Movie>> GetAllAsync()
    {
        return _movieRepository.GetAllAsync();
    }

    public Task<Movie?> GetByIdAsync(Guid id)
    {
        return _movieRepository.GetByIdAsync(id);
    }

    public Task<Movie?> GetBySlugAsync(string slug)
    {
        return _movieRepository.GetBySlugAsync(slug);
    }

    public async Task<Movie?> UpdateAsync(Movie movie)
    {
        var movieExists = await _movieRepository.ExistsById(movie.Id);
        if (!movieExists)
            return null;

        await _movieRepository.UpdateAsync(movie);

        return movie;
    }
}
