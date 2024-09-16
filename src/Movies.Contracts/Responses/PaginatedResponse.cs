namespace Movies.Contracts.Responses;

public record PaginatedResponse<TResponseType>
{
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int Count { get; init; }
    public bool HasNextPage => Count > (Page * PageSize);
    public required IEnumerable<TResponseType> Items { get; init; }
}
