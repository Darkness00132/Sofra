using FluentValidation;

namespace Sofra.Application.Features.Menus.UpdateMenuType;

public sealed class UpdateMenuTypeCommandValidator : AbstractValidator<UpdateMenuTypeCommand>
{
    public UpdateMenuTypeCommandValidator()
    {
        RuleFor(x => x.MenuTypeId)
            .NotEmpty().WithMessage("Menu type id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Menu type name is required.")
            .MaximumLength(100).WithMessage("Menu type name must be at most 100 characters.");
    }
}
