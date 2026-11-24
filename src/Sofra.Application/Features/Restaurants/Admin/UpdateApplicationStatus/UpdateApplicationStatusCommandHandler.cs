using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Constants;
using Sofra.Application.Features.Restaurants.Dtos;
using Sofra.Domain.Entities;
using Sofra.Domain.Enums;
using Sofra.Domain.Exceptions;

namespace Sofra.Application.Features.Restaurants.Admin.UpdateApplicationStatus;

public class UpdateApplicationStatusCommandHandler(
    UserManager<AppUser> userManager,
    ICurrentUser currentUser,
    IRestaurantApplicationRepository applications,
    IRepository<Restaurant> restaurants,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateApplicationStatusCommand, Result<ApplicationResponse>>
{
    public async Task<Result<ApplicationResponse>> Handle(UpdateApplicationStatusCommand request, CancellationToken cancellationToken)
    {
        var adminId = currentUser.GetUserId();

        var application = await applications.FindWithApplicantAsync(request.ApplicationId, cancellationToken);
        if (application is null)
            return Result<ApplicationResponse>.Failure(Error.NotFound(
                "APPLICATION_NOT_FOUND",
                "The restaurant application was not found."));

        if (request.Status is not (RestaurantStatus.Approved or RestaurantStatus.Rejected))
            return Result<ApplicationResponse>.Failure(Error.Validation(
                "STATUS_NOT_SUPPORTED",
                "Only Approved or Rejected are supported for applications."));

        try
        {
            if (request.Status == RestaurantStatus.Approved)
                application.Approve(adminId);
            else
                application.Reject(request.Reason!, adminId);
        }
        catch (DomainException ex)
        {
            return Result<ApplicationResponse>.Failure(Error.BusinessRule("APPLICATION_NOT_PENDING", ex.Message));
        }

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        IdentityResult? roleAssignment = null;

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);

            if (request.Status == RestaurantStatus.Approved)
            {
                // BR-01: the restaurant only comes into existence from the approved application.
                // Guid ids are generated in the domain, so no intermediate save is needed.
                var restaurant = Restaurant.FromApprovedApplication(application);
                await restaurants.AddAsync(restaurant, cancellationToken);
                restaurant.LinkApprovedApplication(application);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                if (application.Applicant is not null)
                    roleAssignment = await userManager.AddToRoleAsync(application.Applicant, Roles.Restaurant);
            }

            if (roleAssignment is not null && !roleAssignment.Succeeded)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ApplicationResponse>.Failure(Error.Internal(
                    "ROLE_ASSIGNMENT_FAILED",
                    "The applicant could not be assigned the Restaurant role."));
            }

            await unitOfWork.CommitTransactionAsync(cancellationToken);

            return application.Adapt<ApplicationResponse>() with
            {
                ApplicantEmail = application.Applicant?.Email ?? string.Empty,
            };
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
