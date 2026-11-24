using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Discovery.Dtos;

namespace Sofra.Application.Features.Discovery.GetRestaurantDetails;

/// <summary>Public details of one approved restaurant (FR-07, FR-10).</summary>
public record GetRestaurantDetailsQuery(Guid RestaurantId) : IRequest<Result<PublicRestaurantResponse>>;
