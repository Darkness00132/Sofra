using Mapster;
using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Restaurants.UploadRestaurantImage;

public class UploadRestaurantImageCommandHandler(
    ICurrentUser currentUser,
    IRestaurantOwnershipGuard ownershipGuard,
    IFileStorage fileStorage,
    IUnitOfWork unitOfWork) : IRequestHandler<UploadRestaurantImageCommand, Result<MyRestaurantResponse>>
{
    public async Task<Result<MyRestaurantResponse>> Handle(UploadRestaurantImageCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var restaurantResult = await ownershipGuard.GetOwnedRestaurantAsync(userId, cancellationToken);
        if (restaurantResult.IsFailure)
            return Result<MyRestaurantResponse>.Failure(restaurantResult.Error!);

        var restaurant = restaurantResult.Response!;

        var previousImagePublicId = restaurant.ImagePublicId;

        var stored = await fileStorage.UploadAsync(request.Image, cancellationToken);

        restaurant.SetImage(stored.Url, stored.StorageId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (previousImagePublicId is not null)
            await fileStorage.DeleteAsync(previousImagePublicId, cancellationToken);

        return restaurant.Adapt<MyRestaurantResponse>();
    }
}
