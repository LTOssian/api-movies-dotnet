namespace Movies.Contracts.Requests;

public record GetAllMoviesRequest : PaginatedRequest
{
    public string? Title { get; init; }
    public int? Year { get; init; }
    public string? SortBy { get; init; }
}
