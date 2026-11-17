using Sofra.Domain.Common;
using Sofra.Domain.Exceptions;

namespace Sofra.Domain.Entities;

/// <summary>
/// A restaurant's own grouping under a platform <see cref="FoodCategory"/>
/// (e.g. "Beef Burgers" under the platform category "Burger").
/// </summary>
public class FoodType : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public Guid FoodCategoryId { get; private set; }

    public FoodCategory FoodCategory { get; private set; } = null!;

    public Guid RestaurantId { get; private set; }

    public Restaurant Restaurant { get; private set; } = null!;

    public ICollection<FoodItem> FoodItems { get; private set; } = [];

    private FoodType() { }

    public static FoodType Create(Guid restaurantId, Guid foodCategoryId, string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Food type name is required.");

        return new FoodType
        {
            RestaurantId = restaurantId,
            FoodCategoryId = foodCategoryId,
            Name = name.Trim(),
            Description = description,
        };
    }

    public void UpdateInfo(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Food type name is required.");

        Name = name.Trim();
        Description = description;
    }
}
