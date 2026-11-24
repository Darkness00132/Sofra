using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Common.Storage;
using Sofra.Application.Features.Menus.Dtos;

namespace Sofra.Application.Features.Menus.UploadMenuItemImage;

/// <summary>Uploads a picture for one of the owner's food items and stores it in file storage (FR-15).</summary>
public record UploadMenuItemImageCommand(Guid FoodItemId, FileDto Image) : IRequest<Result<MenuItemResponse>>;
