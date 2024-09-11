using FluentValidation;
using Movies.Application.MovieUseCases.Validators;
using Movies.Core.Entities;

namespace Movies.Application.MovieUseCases.Services;

public class MovieService : IMovieService
{
    private readonly MovieValidator _movieValidator;
    private readonly IMovieRepository _movieRepository;

    public MovieService(IMovieRepository movieRepository, MovieValidator movieValidator)
    {
        _movieValidator = movieValidator;
        _movieRepository = movieRepository;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, token);

        return await _movieRepository.CreateAsync(movie, token);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
    {
        return _movieRepository.DeleteAsync(id, token);
    }

    public Task<bool> ExistsById(Guid id, CancellationToken token = default)
    {
        return _movieRepository.ExistsById(id, token);
    }

    public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default)
    {
        return _movieRepository.GetAllAsync(token);
    }

    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return _movieRepository.GetByIdAsync(id, token);
    }

    public Task<Movie?> GetBySlugAsync(string slug, CancellationToken token = default)
    {
        return _movieRepository.GetBySlugAsync(slug, token);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, CancellationToken token = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, token);

        var movieExists = await _movieRepository.ExistsById(movie.Id, token);
        if (!movieExists)
            return null;

        await _movieRepository.UpdateAsync(movie, token);

        return movie;
    }
}
