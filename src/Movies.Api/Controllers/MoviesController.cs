using System;
using Microsoft.AspNetCore.Mvc;
using Movies.Application.MovieUseCases;
using Movies.Core.Entities;

namespace Movies.Api.Controllers;

[Route("/api/movies")]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieRepository _movieRepository;

    public MoviesController(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody]CreateMovieRequest request)
    {
        var movie = request.ToMovie();

        await _movieRepository.CreateAsync(movie);

        return Ok(request.ToResponse(movie));
    }
}
