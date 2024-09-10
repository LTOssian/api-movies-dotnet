using Movies.Application.MovieUseCases;
using Movies.Core.Entities;

namespace Movies.Infrastructure.Repositories.InMemory;

public class MovieRepository : IMovieRepository
{
    private readonly List<Movie> _movies = [];

    public Task<bool> CreateAsync(Movie movie)
    {
        try
        {
            _movies.Add(movie);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var removed = _movies.RemoveAll(movie => movie.Id == id);

        return Task.FromResult(removed > 0);
    }

    public Task<IEnumerable<Movie>> GetAllAsync()
    {
        return Task.FromResult(_movies.AsEnumerable());
    }

    public Task<Movie?> GetByIdAsync(Guid id)
    {
        var movie = _movies.SingleOrDefault(movie => movie.Id == id);
        return Task.FromResult(movie);
    }

    public Task<Movie?> GetBySlugAsync(string slug)
    {
        var movie = _movies.SingleOrDefault(movie => movie.Slug == slug);
        return Task.FromResult(movie);
    }

    public Task<bool> UpdateAsync(Movie movie)
    {
        var movieIndex = _movies.FindIndex(item => item.Id == movie.Id);
        if (movieIndex == -1)
            return Task.FromResult(false);

        _movies[movieIndex] = movie;

        return Task.FromResult(true);
    }
}
