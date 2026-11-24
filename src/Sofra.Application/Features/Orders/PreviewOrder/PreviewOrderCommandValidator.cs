using FluentValidation;
using Sofra.Domain.Enums;

namespace Sofra.Application.Features.Orders.PreviewOrder;

public sealed class PreviewOrderCommandValidator : AbstractValidator<PreviewOrderCommand>
{
    public PreviewOrderCommandValidator()
    {
        RuleFor(x => x.FulfilmentType)
            .IsInEnum().WithMessage("Fulfilment type is not valid.");

        RuleFor(x => x.DeliveryAddress)
            .NotEmpty().WithMessage("Delivery address is required for delivery orders.")
            .When(x => x.FulfilmentType == FulfilmentType.Delivery);
    }
}
