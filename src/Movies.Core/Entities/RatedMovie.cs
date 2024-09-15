namespace Movies.Core.Entities;

public class RatedMovie
{
    public required Guid MovieId { get; init; }
    public required string Slug { get; init; }
    public required int Rating { get; init; }
}
