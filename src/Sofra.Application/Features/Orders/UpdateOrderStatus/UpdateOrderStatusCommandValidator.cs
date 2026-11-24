using FluentValidation;
using Sofra.Domain.Enums;

namespace Sofra.Application.Features.Orders.UpdateOrderStatus;

public sealed class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    private static readonly OrderStatus[] SupportedStatuses =
        [OrderStatus.Accepted, OrderStatus.Rejected, OrderStatus.Preparing, OrderStatus.Ready, OrderStatus.Completed];

    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order id is required.");

        RuleFor(x => x.Status)
            .Must(s => SupportedStatuses.Contains(s))
            .WithMessage("Status must be Accepted, Rejected, Preparing, Ready or Completed.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("A rejection reason is required.")
            .When(x => x.Status == OrderStatus.Rejected);
    }
}
