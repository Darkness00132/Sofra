using Sofra.Domain.Common;
using Sofra.Domain.Exceptions;

namespace Sofra.Domain.Entities;

/// <summary>Platform-level category (e.g. Burger, Pizza) that all restaurants use.</summary>
public class FoodCategory : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public ICollection<FoodType> FoodTypes { get; private set; } = [];

    private FoodCategory() { }

    public static FoodCategory Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Food category name is required.");

        return new FoodCategory
        {
            Name = name.Trim(),
            Description = description,
        };
    }

    public void UpdateInfo(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Food category name is required.");

        Name = name.Trim();
        Description = description;
    }
}
