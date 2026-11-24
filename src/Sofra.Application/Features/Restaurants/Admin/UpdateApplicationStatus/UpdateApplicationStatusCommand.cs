using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;
using Sofra.Domain.Enums;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Restaurants.Admin.UpdateApplicationStatus;

/// <summary>
/// Decides a restaurant application. Approving materializes the restaurant and grants the
/// applicant the Restaurant role; rejecting requires a reason.
/// </summary>
public record UpdateApplicationStatusCommand(
    Guid ApplicationId,
    RestaurantStatus Status,
    string? Reason) : IRequest<Result<ApplicationResponse>>;
