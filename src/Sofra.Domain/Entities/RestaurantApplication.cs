using Sofra.Domain.Common;
using Sofra.Domain.Enums;
using Sofra.Domain.Exceptions;

namespace Sofra.Domain.Entities;

public class RestaurantApplication : Entity
{
    public Guid ApplicantId { get; private set; }

    public AppUser Applicant { get; private set; } = null!;

    public string RestaurantName { get; private set; } = string.Empty;

    public string Address { get; private set; } = string.Empty;

    public string Phone { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public string? Documents { get; private set; }

    public RestaurantStatus Status { get; private set; }

    public string? RejectionReason { get; private set; }

    public DateTime? ReviewedAt { get; private set; }

    public Guid? ReviewedByUserId { get; private set; }

    public AppUser? ReviewedByUser { get; private set; }

    public Guid? RestaurantId { get; private set; }

    public Restaurant? Restaurant { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private RestaurantApplication() { }

    public static RestaurantApplication Create(
        Guid applicantId,
        string restaurantName,
        string address,
        string phone,
        string description,
        string? documents = null)
    {
        if (string.IsNullOrWhiteSpace(restaurantName))
            throw new DomainException("Restaurant name is required.");

        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("Restaurant address is required.");

        return new RestaurantApplication
        {
            ApplicantId = applicantId,
            RestaurantName = restaurantName,
            Address = address,
            Phone = phone,
            Description = description,
            Documents = documents,
            Status = RestaurantStatus.PendingApproval,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void Approve(Guid reviewedBy)
    {
        EnsurePendingApproval(nameof(Approve));
        Status = RestaurantStatus.Approved;
        SetReview(reviewedBy);
    }

    public void Reject(string reason, Guid reviewedBy)
    {
        EnsurePendingApproval(nameof(Reject));
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("A rejection reason is required.");

        Status = RestaurantStatus.Rejected;
        RejectionReason = reason;
        SetReview(reviewedBy);
    }

    /// <summary>Called once the Restaurant entity is materialized from an approved application.</summary>
    public void LinkRestaurant(Guid restaurantId)
    {
        if (Status != RestaurantStatus.Approved)
            throw new DomainException("Only an approved application can be linked to a restaurant.");

        RestaurantId = restaurantId;
    }

    private void EnsurePendingApproval(string operation)
    {
        if (Status != RestaurantStatus.PendingApproval)
            throw new DomainException($"Application is already {Status}; cannot {operation}.");
    }

    private void SetReview(Guid reviewedBy)
    {
        ReviewedAt = DateTime.UtcNow;
        ReviewedByUserId = reviewedBy;
    }
}
