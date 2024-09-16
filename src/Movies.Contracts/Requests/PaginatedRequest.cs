namespace Movies.Contracts.Requests;

public record PaginatedRequest
{
    public int? Page { get; init; } = 1;
    public int? PageSize { get; init; } = 10;
}
