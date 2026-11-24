using FluentValidation;
using Sofra.Domain.Enums;

namespace Sofra.Application.Features.Orders.PlaceOrder;

public sealed class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(x => x.FulfilmentType)
            .IsInEnum().WithMessage("Fulfilment type is not valid.");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("Payment method is not valid.");

        RuleFor(x => x.DeliveryAddress)
            .NotEmpty().WithMessage("Delivery address is required for delivery orders.")
            .When(x => x.FulfilmentType == FulfilmentType.Delivery);
    }
}
