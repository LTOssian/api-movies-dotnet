using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.MovieUseCases.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers.V1;

[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPost(ApiEndpoints.V1.Movies.Create)]
    public async Task<IActionResult> Create(
        [FromBody] CreateMovieRequest request,
        CancellationToken token
    )
    {
        var movie = request.ToMovie();

        await _movieService.CreateAsync(movie, token);

        var response = request.ToResponse(movie);
        return CreatedAtAction(nameof(Get), new { idOrSlug = response.Id }, response);
    }

    [HttpGet(ApiEndpoints.V1.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieService.GetByIdAsync(id, userId, token)
            : await _movieService.GetBySlugAsync(idOrSlug, userId, token);

        if (movie is null)
        {
            return NotFound();
        }

        var response = movie.ToResponse();

        return Ok(response);
    }

    [HttpGet(ApiEndpoints.V1.Movies.GetAll)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllMoviesRequest request,
        CancellationToken token
    )
    {
        var userId = HttpContext.GetUserId();
        var options = request.ToOptions().WithUserId(userId);

        var movies = await _movieService.GetAllAsync(options, token);
        var moviesCount = await _movieService.GetCountAsync(options.Title, options.YearOfRelease, token);

        var response = movies.ToResponse(request.Page, request.PageSize, moviesCount);

        return Ok(response);
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPut(ApiEndpoints.V1.Movies.Update)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateMovieRequest request,
        CancellationToken token
    )
    {
        var userId = HttpContext.GetUserId();

        var movie = request.ToMovie(id);
        var updatedMovie = await _movieService.UpdateAsync(movie, userId, token);
        if (updatedMovie is null)
        {
            return NotFound();
        }

        var response = updatedMovie.ToResponse();

        return Ok(response);
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.V1.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        var deleted = await _movieService.DeleteAsync(id, token);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
