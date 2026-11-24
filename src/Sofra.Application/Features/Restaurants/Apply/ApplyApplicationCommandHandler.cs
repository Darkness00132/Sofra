using Mapster;
using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;
using Sofra.Domain.Entities;
using Sofra.Domain.Enums;

namespace Sofra.Application.Features.Restaurants.Apply;

public class ApplyApplicationCommandHandler(
    ICurrentUser currentUser,
    IAccountRepository accounts,
    IRepository<RestaurantApplication> applications,
    IRepository<Restaurant> restaurants,
    IUnitOfWork unitOfWork) : IRequestHandler<ApplyApplicationCommand, Result<ApplicationResponse>>
{
    public async Task<Result<ApplicationResponse>> Handle(ApplyApplicationCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        // Both queries run on the same scoped DbContext, which forbids
        // overlapping operations (Task.WhenAll would trip the concurrency
        // detector), so run them sequentially.
        var hasPendingApplication = await applications.ExistsAsync(
            a => a.ApplicantId == userId && a.Status == RestaurantStatus.PendingApproval, cancellationToken);
        var ownsRestaurant = await restaurants.ExistsAsync(r => r.OwnerId == userId, cancellationToken);

        if (hasPendingApplication)
            return Result<ApplicationResponse>.Failure(Error.Conflict(
                "APPLICATION_ALREADY_PENDING",
                "You already have an application waiting for review."));

        if (ownsRestaurant)
            return Result<ApplicationResponse>.Failure(Error.Conflict(
                "RESTAURANT_ALREADY_EXISTS",
                "You already own a restaurant."));

        var application = RestaurantApplication.Create(
            userId,
            request.RestaurantName,
            request.Address,
            request.Phone,
            request.Description);

        await applications.AddAsync(application, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var applicantEmail = await accounts.GetEmailAsync(userId, cancellationToken);

        return application.Adapt<ApplicationResponse>() with
        {
            ApplicantEmail = applicantEmail ?? string.Empty,
        };
    }
}
