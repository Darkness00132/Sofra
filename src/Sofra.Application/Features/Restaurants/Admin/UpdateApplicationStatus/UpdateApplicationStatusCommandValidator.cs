using FluentValidation;
using Sofra.Domain.Enums;

namespace Sofra.Application.Features.Restaurants.Admin.UpdateApplicationStatus;

public sealed class UpdateApplicationStatusCommandValidator : AbstractValidator<UpdateApplicationStatusCommand>
{
    public UpdateApplicationStatusCommandValidator()
    {
        RuleFor(x => x.Status)
            .Must(s => s is RestaurantStatus.Approved or RestaurantStatus.Rejected)
            .WithMessage("Status must be Approved or Rejected.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("A rejection reason is required.")
            .When(x => x.Status == RestaurantStatus.Rejected);
    }
}
