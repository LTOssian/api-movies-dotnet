using FluentValidation;

namespace Movies.Application.RatingUseCases.Validators;

public class RatingValidator : AbstractValidator<int>
{
    public RatingValidator()
    {
        RuleFor(x => x).InclusiveBetween(0, 5);
    }
}
