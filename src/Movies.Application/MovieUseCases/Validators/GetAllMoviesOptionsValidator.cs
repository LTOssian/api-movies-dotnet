using FluentValidation;

namespace Movies.Application.MovieUseCases.Validators;

public class GetAllMoviesOptionsValidator : AbstractValidator<GetAllMoviesOptions>
{
    private static readonly string[] AcceptableSortFields = { "year_of_release", "title" };

    public GetAllMoviesOptionsValidator()
    {
        RuleFor(x => x.YearOfRelease).LessThanOrEqualTo(DateTime.UtcNow.Year);
        RuleFor(x => x.SortField)
            .Must(x =>
                x is null || AcceptableSortFields.Contains(x, StringComparer.OrdinalIgnoreCase)
            )
            .WithMessage("You can only sort by title or year_of_release");
        RuleFor(x => x.Page).NotEmpty().GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).NotEmpty().GreaterThanOrEqualTo(1).LessThanOrEqualTo(25);
    }
}
