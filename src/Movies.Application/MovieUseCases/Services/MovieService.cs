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

    public async Task<bool> CreateAsync(Movie movie)
    {
        await _movieValidator.ValidateAndThrowAsync(movie);
        
        return await  _movieRepository.CreateAsync(movie);
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
        await _movieValidator.ValidateAndThrowAsync(movie);

        var movieExists = await _movieRepository.ExistsById(movie.Id);
        if (!movieExists)
            return null;

        await _movieRepository.UpdateAsync(movie);

        return movie;
    }
}
