using System.Text.RegularExpressions;

namespace Movies.Core.Entities;

public partial record Movie
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public string Slug => GenerateSlug();
    public required int YearOfRelease { get; init; }
    public required List<string> Genres { get; init; } = [];

    public float? Ratings { get; init; }
    public int? UserRating { get; init; }

    private string GenerateSlug()
    {
        var slugText = SlugRegex().Replace(Title, string.Empty).ToLower().Replace(" ", "-");

        return $"{slugText}-{YearOfRelease}";
    }

    [GeneratedRegex("[^0-9A-Za-z _-]", RegexOptions.NonBacktracking, 10)]
    private static partial Regex SlugRegex();
}
