using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;

namespace Sofra.Application.Features.Restaurants.Apply;

/// <summary>Submits a restaurant application for administrator review.</summary>
public record ApplyApplicationCommand(
    string RestaurantName,
    string Address,
    string Phone,
    string Description) : IRequest<Result<ApplicationResponse>>;
