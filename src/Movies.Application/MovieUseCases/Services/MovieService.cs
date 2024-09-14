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

    public async Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
    {
        return await _movieRepository.DeleteAsync(id, token);
    }

    public async Task<bool> ExistsById(Guid id, CancellationToken token = default)
    {
        return await _movieRepository.ExistsById(id, token);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(
        Guid? userId,
        CancellationToken token = default
    )
    {
        return await _movieRepository.GetAllAsync(userId, token);
    }

    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId, CancellationToken token = default)
    {
        return await _movieRepository.GetByIdAsync(id, userId, token);
    }

    public async Task<Movie?> GetBySlugAsync(
        string slug,
        Guid? userId,
        CancellationToken token = default
    )
    {
        return await _movieRepository.GetBySlugAsync(slug, userId, token);
    }

    public async Task<Movie?> UpdateAsync(
        Movie movie,
        CancellationToken token = default
    )
    {
        await _movieValidator.ValidateAndThrowAsync(movie, token);

        var movieExists = await _movieRepository.ExistsById(movie.Id, token);
        if (!movieExists)
            return null;

        await _movieRepository.UpdateAsync(movie, token);

        return movie;
    }
}
