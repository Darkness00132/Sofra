using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Domain.Enums;
using Sofra.Application.Features.Menus.Dtos;

namespace Sofra.Application.Features.Menus.AddMenuItem;

/// <summary>Adds a food item to one of the owner's menu types (FR-14, FR-15).</summary>
public record AddMenuItemCommand(
    Guid MenuTypeId,
    string Name,
    string Description,
    decimal Price,
    FoodAvailability Availability = FoodAvailability.Available) : IRequest<Result<MenuItemResponse>>;
