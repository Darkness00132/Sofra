using FluentValidation;

namespace Sofra.Application.Features.Menus.AddMenuType;

public sealed class AddMenuTypeCommandValidator : AbstractValidator<AddMenuTypeCommand>
{
    public AddMenuTypeCommandValidator()
    {
        RuleFor(x => x.FoodCategoryId)
            .NotEmpty().WithMessage("Food category is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Menu type name is required.")
            .MaximumLength(100).WithMessage("Menu type name must be at most 100 characters.");
    }
}
