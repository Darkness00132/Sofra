using FluentValidation;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Restaurants.Apply;

public sealed class ApplyApplicationCommandValidator : AbstractValidator<ApplyApplicationCommand>
{
    public ApplyApplicationCommandValidator()
    {
        RuleFor(x => x.RestaurantName)
            .NotEmpty().WithMessage("Restaurant name is required.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");
    }
}
