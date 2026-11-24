using FluentValidation;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Admin.ListAccounts;

public sealed class ListAccountsQueryValidator : AbstractValidator<ListAccountsQuery>
{
    public ListAccountsQueryValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Filter)
            .NotNull().WithMessage("Filter is required.");

        RuleFor(x => x.Filter.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than zero.");

        RuleFor(x => x.Filter.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.Filter)
            .Must(f => f.IsValidRole())
            .WithMessage("Role filter must be Customer, Restaurant or Administrator.");
    }
}
