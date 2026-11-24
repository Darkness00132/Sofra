using MediatR;
using Sofra.Application.Common.Results;

namespace Sofra.Application.Features.Orders.RestaurantDashboard;

/// <summary>The signed-in owner's sales dashboard (PRD: order history and basic sales information).</summary>
public record RestaurantDashboardQuery : IRequest<Result<OwnerDashboardResponse>>;
