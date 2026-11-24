using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;

namespace Sofra.Application.Features.Restaurants.Admin.GetApplication;

/// <summary>Returns one restaurant application for administrator review.</summary>
public record GetApplicationQuery(Guid ApplicationId) : IRequest<Result<ApplicationResponse>>;
