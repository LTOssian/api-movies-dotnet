using Movies.Application.MovieUseCases;
using Movies.Core.Entities;

namespace Movies.Infrastructure.Repositories.InMemory;

public class MovieRepository : IMovieRepository
{
    private readonly List<Movie> _movies = [];

    public Task<bool> CreateAsync(Movie movie, CancellationToken token)
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

    public Task<bool> DeleteAsync(Guid id, CancellationToken token)
    {
        var removed = _movies.RemoveAll(movie => movie.Id == id);

        return Task.FromResult(removed > 0);
    }

    public Task<bool> ExistsById(Guid id, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token)
    {
        return Task.FromResult(_movies.AsEnumerable());
    }

    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken token)
    {
        var movie = _movies.SingleOrDefault(movie => movie.Id == id);
        return Task.FromResult(movie);
    }

    public Task<Movie?> GetBySlugAsync(string slug, CancellationToken token)
    {
        var movie = _movies.SingleOrDefault(movie => movie.Slug == slug);
        return Task.FromResult(movie);
    }

    public Task<bool> UpdateAsync(Movie movie, CancellationToken token)
    {
        var movieIndex = _movies.FindIndex(item => item.Id == movie.Id);
        if (movieIndex == -1)
            return Task.FromResult(false);

        _movies[movieIndex] = movie;

        return Task.FromResult(true);
    }
}
