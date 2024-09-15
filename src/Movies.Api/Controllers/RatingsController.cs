using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.RatingUseCases.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

public class RatingsController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingsController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    [Authorize]
    [HttpPut(ApiEndpoints.Movies.Rate)]
    public async Task<IActionResult> RateMovie(
        [FromRoute] Guid id,
        [FromBody] RateMovieRequest request,
        CancellationToken token
    )
    {
        var userId = HttpContext.GetUserId();
        var result = await _ratingService.RateMovieAsync(request.Rating, id, userId!.Value, token);

        return result ? Ok() : NotFound();
    }

    [Authorize]
    [HttpDelete(ApiEndpoints.Movies.DeleteRating)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var deleted = await _ratingService.DeleteRatingAsync(id, userId!.Value, token);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [Authorize]
    [HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
    public async Task<IActionResult> Get(CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var ratings = await _ratingService.GetRatedMovies(userId!.Value, token);
        var response = ratings.Select(x => x.ToResponse());

        return Ok(response);
    }
}
