namespace Sofra.Application.Features.Restaurants.Dtos;

/// <summary>A restaurant application as seen by applicants and administrators.</summary>
public record ApplicationResponse(
    Guid Id,
    string RestaurantName,
    string Address,
    string Phone,
    string Description,
    string Status,
    string? RejectionReason,
    DateTime CreatedAt,
    DateTime? ReviewedAt,
    string ApplicantEmail);
