using FluentValidation;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Restaurants.UpdateMyRestaurant;

public sealed class UpdateMyRestaurantCommandValidator : AbstractValidator<UpdateMyRestaurantCommand>
{
    public UpdateMyRestaurantCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Restaurant name is required.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");
    }
}
