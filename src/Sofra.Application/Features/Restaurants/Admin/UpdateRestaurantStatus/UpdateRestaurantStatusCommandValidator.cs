using FluentValidation;
using Sofra.Domain.Enums;

namespace Sofra.Application.Features.Restaurants.Admin.UpdateRestaurantStatus;

public sealed class UpdateRestaurantStatusCommandValidator : AbstractValidator<UpdateRestaurantStatusCommand>
{
    public UpdateRestaurantStatusCommandValidator()
    {
        RuleFor(x => x.Status)
            .Must(s => s is RestaurantStatus.Suspended or RestaurantStatus.Approved)
            .WithMessage("Status must be Suspended or Approved.");
    }
}
