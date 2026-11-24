using Mapster;
using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Restaurants.RemoveRestaurantImage;

public class RemoveRestaurantImageCommandHandler(
    ICurrentUser currentUser,
    IRestaurantOwnershipGuard ownershipGuard,
    IFileStorage fileStorage,
    IUnitOfWork unitOfWork) : IRequestHandler<RemoveRestaurantImageCommand, Result<MyRestaurantResponse>>
{
    public async Task<Result<MyRestaurantResponse>> Handle(RemoveRestaurantImageCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var restaurantResult = await ownershipGuard.GetOwnedRestaurantAsync(userId, cancellationToken);
        if (restaurantResult.IsFailure)
            return Result<MyRestaurantResponse>.Failure(restaurantResult.Error!);

        var restaurant = restaurantResult.Response!;

        if (restaurant.ImagePublicId is null)
            return Result<MyRestaurantResponse>.Failure(Error.BusinessRule(
                "RESTAURANT_IMAGE_NOT_FOUND",
                "The restaurant has no picture."));

        var removedImagePublicId = restaurant.ImagePublicId;

        restaurant.ClearImage();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await fileStorage.DeleteAsync(removedImagePublicId, cancellationToken);

        return restaurant.Adapt<MyRestaurantResponse>();
    }
}
