using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Domain.Enums;

namespace Sofra.Application.Features.Orders.PreviewOrder;

/// <summary>
/// Computes the exact charges the checkout would produce for the current cart — without
/// placing the order (FR-25: review the complete order before submission).
/// </summary>
public record PreviewOrderCommand(
    FulfilmentType FulfilmentType,
    string? DeliveryAddress) : IRequest<Result<OrderPreviewResponse>>;
