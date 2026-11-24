namespace Sofra.Application.Features.Menus.Dtos;

/// <summary>A food item on a menu.</summary>
public record MenuItemResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Availability,
    bool IsAvailable,
    string? ImageUrl);
