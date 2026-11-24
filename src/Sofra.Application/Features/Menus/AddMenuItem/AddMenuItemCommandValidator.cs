using FluentValidation;

namespace Sofra.Application.Features.Menus.AddMenuItem;

public sealed class AddMenuItemCommandValidator : AbstractValidator<AddMenuItemCommand>
{
    public AddMenuItemCommandValidator()
    {
        RuleFor(x => x.MenuTypeId)
            .NotEmpty().WithMessage("Menu type is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Food item name is required.")
            .MaximumLength(200).WithMessage("Food item name must be at most 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Food item description is required.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
    }
}
