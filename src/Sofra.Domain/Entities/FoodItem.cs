using Sofra.Domain.Common;
using Sofra.Domain.Enums;
using Sofra.Domain.Exceptions;

namespace Sofra.Domain.Entities;

public class FoodItem : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public string? ImageUrl { get; private set; }

    public string? ImagePublicId { get; private set; }

    public FoodAvailability Availability { get; private set; }

    public Guid FoodTypeId { get; private set; }

    public FoodType FoodType { get; private set; } = null!;

    public ICollection<OrderItem> OrderItems { get; private set; } = [];
    public ICollection<CartItem> CartItems { get; private set; } = [];

    private FoodItem() { }

    public static FoodItem Create(
        Guid foodTypeId,
        string name,
        string description,
        decimal price,
        FoodAvailability availability = FoodAvailability.Available)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Food item name is required.");

        if (price < 0)
            throw new DomainException("Food item price cannot be negative.");

        return new FoodItem
        {
            FoodTypeId = foodTypeId,
            Name = name.Trim(),
            Description = description,
            Price = price,
            Availability = availability,
        };
    }

    /// <summary>
    /// Creates the item together with its <see cref="FoodType"/> so the object graph is usable
    /// without EF navigation fixup (in-memory composition, unit tests).
    /// </summary>
    public static FoodItem Create(
        FoodType foodType,
        string name,
        string description,
        decimal price,
        FoodAvailability availability = FoodAvailability.Available)
    {
        var item = Create(foodType.Id, name, description, price, availability);
        item.FoodType = foodType;
        return item;
    }

    public void UpdateInfo(string name, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Food item name is required.");

        if (price < 0)
            throw new DomainException("Food item price cannot be negative.");

        Name = name.Trim();
        Description = description;
        Price = price;
    }

    public void MakeAvailable() => Availability = FoodAvailability.Available;

    public void MakeUnavailable() => Availability = FoodAvailability.Unavailable;

    /// <summary>Points the item at its stored picture. The file itself is managed by the file storage service.</summary>
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

    public bool IsAvailable => Availability == FoodAvailability.Available;
}
