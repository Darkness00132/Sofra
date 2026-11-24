using FluentValidation;

namespace Sofra.Application.Features.Menus.UpdateMenuItem;

public sealed class UpdateMenuItemCommandValidator : AbstractValidator<UpdateMenuItemCommand>
{
    public UpdateMenuItemCommandValidator()
    {
        RuleFor(x => x.FoodItemId)
            .NotEmpty().WithMessage("Food item id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Food item name is required.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Food item description is required.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
    }
}
