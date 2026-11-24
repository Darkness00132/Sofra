using FluentValidation;

namespace Sofra.Application.Features.Orders.ListRestaurantOrders;

public sealed class ListRestaurantOrdersQueryValidator : AbstractValidator<ListRestaurantOrdersQuery>
{
    public ListRestaurantOrdersQueryValidator()
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
