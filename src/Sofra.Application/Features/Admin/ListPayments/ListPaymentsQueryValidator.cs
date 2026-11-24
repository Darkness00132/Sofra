using FluentValidation;

namespace Sofra.Application.Features.Admin.ListPayments;

public sealed class ListPaymentsQueryValidator : AbstractValidator<ListPaymentsQuery>
{
    public ListPaymentsQueryValidator()
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
