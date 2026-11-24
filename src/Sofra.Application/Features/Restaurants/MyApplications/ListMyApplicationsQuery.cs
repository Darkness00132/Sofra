using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;

namespace Sofra.Application.Features.Restaurants.MyApplications;

/// <summary>Lists the signed-in user's own applications, newest first.</summary>
public record ListMyApplicationsQuery : IRequest<Result<IReadOnlyList<ApplicationResponse>>>;
