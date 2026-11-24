using FluentValidation;

namespace Sofra.Application.Features.Admin.ListOrders;

public sealed class ListOrdersQueryValidator : AbstractValidator<ListOrdersQuery>
{
    public ListOrdersQueryValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Filter)
            .NotNull().WithMessage("Filter is required.");

        RuleFor(x => x.Filter.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than zero.");

        RuleFor(x => x.Filter.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.Filter)
            .Must(f => f.From is null || f.To is null || f.From <= f.To)
            .WithMessage("From date must be before the To date.");
    }
}
