using FluentAssertions;
using Movies.Application.MovieUseCases;
using Movies.Application.MovieUseCases.Validators;
using Movies.Core.Entities;

namespace Movies.UnitTests;

public class MovieSpecification
{
    [Fact]
    public void Should_Create_Movie_With_Valid_Properties()
    {
        Guid movieId = Guid.NewGuid();
        string title = "Le Comte de Monte Cristo";
        int yearOfRelease = 2024;
        var genres = new List<string> { "Action", "Suspense" };

        var movie = new Movie
        {
            Id = movieId,
            Title = title,
            YearOfRelease = yearOfRelease,
            Genres = genres
        };

        movie.Id.Should().Be(movieId);
        movie.Title.Should().Be(movie.Title);
        movie.YearOfRelease.Should().Be(yearOfRelease);
        movie.Genres.Should().ContainInOrder(genres);
    }

    [Theory]
    [InlineData("Le Comte de Monte Cristo", 2024, "le-comte-de-monte-cristo-2024")]
    [InlineData("Star Wars: Episode IV - A New Hope!", 1977, "star-wars-episode-iv---a-new-hope-1977")]
    public void Should_Generate_Valid_Slug(string title, int yearOfRelease, string expectedSlug)
    {
        var movie = new Movie{
            Id= Guid.NewGuid(),
            Title = title,
            YearOfRelease = yearOfRelease,
            Genres = new List<string> { "Action "}
        };

        movie.Slug.Should().Be(expectedSlug);
    }

    [Fact]
    public async Task Should_Validate_Movie_With_Valid_Properties()
    {
        var movie = CreateValidMovie();
        var validator = new MovieValidator(new StubFailingGetBySlugRepository());

        var validationResult = await validator.ValidateAsync(movie, default);

        validationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Throw_Movie_With_Invalid_Properties()
    {
        var movie = CreateInvalidMovie();
        var validator = new MovieValidator(new StubFailingGetBySlugRepository());

        var validationResult = await validator.ValidateAsync(movie, default);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(4);
        validationResult.Errors.Should().ContainSingle(e => e.PropertyName == nameof(Movie.Id));
        validationResult.Errors.Should().ContainSingle(e => e.PropertyName == nameof(Movie.Title));
        validationResult.Errors.Should().ContainSingle(e => e.PropertyName == nameof(Movie.YearOfRelease));
        validationResult.Errors.Should().ContainSingle(e => e.PropertyName == nameof(Movie.Genres));
    }



    private static Movie CreateValidMovie()
    {
        return new Movie
        {
            Id = Guid.NewGuid(),
            Title = "Le Comte de Monte Cristo",
            YearOfRelease = 2024,
            Genres = new List<string> { "Action", "Suspense" }
        };
    }

    private static Movie CreateInvalidMovie()
    {
        return new Movie
        {
            Id = Guid.Empty,
            Title = "",
            YearOfRelease = DateTime.Now.AddYears(6).Year,
            Genres = null
        };
    }
}

internal class StubFailingGetBySlugRepository : IMovieRepository
{
    public Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsById(Guid id, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Movie>> GetAllAsync(Guid? userId, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<Movie?> GetByIdAsync(Guid id, Guid? userId, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId, CancellationToken token = default)
    {
        return await Task.FromResult<Movie?>(null);
    }

    public Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
