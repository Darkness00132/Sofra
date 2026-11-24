using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Common.Storage;
using Sofra.Application.Features.Restaurants.Dtos;

namespace Sofra.Application.Features.Restaurants.UploadRestaurantImage;

/// <summary>Uploads a picture for the owner's restaurant and stores it in file storage (FR-12).</summary>
public record UploadRestaurantImageCommand(FileDto Image) : IRequest<Result<MyRestaurantResponse>>;
