using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Restaurants.MyApplications;

public class ListMyApplicationsQueryHandler(
    ICurrentUser currentUser,
    IRestaurantApplicationRepository applications,
    UserManager<AppUser> userManager) : IRequestHandler<ListMyApplicationsQuery, Result<IReadOnlyList<ApplicationResponse>>>
{
    public async Task<Result<IReadOnlyList<ApplicationResponse>>> Handle(ListMyApplicationsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var userApplications = (await applications.ListAsync(a => a.ApplicantId == userId, cancellationToken))
            .OrderByDescending(a => a.CreatedAt)
            .ToList();

        var applicantEmail = (await userManager.FindByIdAsync(userId.ToString()))?.Email ?? string.Empty;

        var items = userApplications.Adapt<List<ApplicationResponse>>()
            .Select(i => i with { ApplicantEmail = applicantEmail })
            .ToList();

        return Result<IReadOnlyList<ApplicationResponse>>.Success(items);
    }
}
