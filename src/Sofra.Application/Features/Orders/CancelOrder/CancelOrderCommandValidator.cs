using FluentValidation;
using Sofra.Domain.Enums;

namespace Sofra.Application.Features.Orders.CancelOrder;

public sealed class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order id is required.");

        RuleFor(x => x.Reason)
            .IsInEnum().WithMessage("Cancellation reason is not valid.");
    }
}
