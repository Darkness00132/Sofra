using FluentValidation;
using Sofra.Application.Features.Restaurants.Admin.ListRestaurants;

namespace Sofra.Application.Features.Discovery.ListRestaurants;

public sealed class ListRestaurantsQueryValidator : AbstractValidator<ListRestaurantsQuery>
{
    public ListRestaurantsQueryValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Filter)
            .NotNull().WithMessage("Filter is required.");

        RuleFor(x => x.Filter.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than zero.");

        RuleFor(x => x.Filter.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}
