using Sofra.Domain.Common;
using Sofra.Domain.Enums;
using Sofra.Domain.Exceptions;

namespace Sofra.Domain.Entities;

public class Restaurant : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;

    public string Address { get; private set; } = string.Empty;

    public string Phone { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public RestaurantStatus Status { get; private set; }

    public Guid OwnerId { get; private set; }

    public AppUser? Owner { get; private set; }

    public bool IsAcceptingOrders { get; private set; }

    public string? ImageUrl { get; private set; }

    public string? ImagePublicId { get; private set; }

    public ICollection<FoodType> FoodTypes { get; private set; } = [];
    public ICollection<Order> Orders { get; private set; } = [];
    public ICollection<RestaurantApplication> Applications { get; private set; } = [];

    private Restaurant() { }

    /// <summary>A restaurant only becomes real after its application is approved.</summary>
    public static Restaurant FromApprovedApplication(RestaurantApplication application)
    {
        if (application.Status != RestaurantStatus.Approved)
            throw new DomainException("A restaurant can only be created from an approved application.");

        return new Restaurant
        {
            Name = application.RestaurantName,
            Address = application.Address,
            Phone = application.Phone,
            Description = application.Description,
            Status = RestaurantStatus.Approved,
            OwnerId = application.ApplicantId,
            IsAcceptingOrders = true,
        };
    }

    /// <summary>Call AFTER saving the restaurant so it has a real id.</summary>
    public void LinkApprovedApplication(RestaurantApplication application)
    {
        application.LinkRestaurant(Id);
    }

    public void UpdateInfo(string name, string address, string phone, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Restaurant name is required.");

        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("Restaurant address is required.");

        Name = name.Trim();
        Address = address;
        Phone = phone;
        Description = description;
    }

    public void SetAcceptingOrders(bool accepting) => IsAcceptingOrders = accepting;

    /// <summary>Points the restaurant at its stored picture. The file itself is managed by the file storage service.</summary>
    public void SetImage(string imageUrl, string? publicId)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new DomainException("Image URL is required.");

        ImageUrl = imageUrl;
        ImagePublicId = publicId;
    }

    /// <summary>Removes the picture reference (the stored file is deleted by the file storage service).</summary>
    public void ClearImage()
    {
        ImageUrl = null;
        ImagePublicId = null;
    }

    public void Suspend()
    {
        if (Status != RestaurantStatus.Approved)
            throw new DomainException($"Restaurant cannot be suspended from {Status} state.");

        Status = RestaurantStatus.Suspended;
        IsAcceptingOrders = false;
    }

    public void Reinstate()
    {
        if (Status != RestaurantStatus.Suspended)
            throw new DomainException($"Restaurant cannot be reinstated from {Status} state.");

        Status = RestaurantStatus.Approved;
        IsAcceptingOrders = true;
    }

    public bool CanAcceptNewOrders => IsAcceptingOrders && Status == RestaurantStatus.Approved;
}
