using Sofra.Domain.Entities;
using Sofra.Domain.Exceptions;

namespace Sofra.DomainTests;

/// <summary>Menu item invariants and availability (SRS FR-15, FR-16, BR-05).</summary>
public class FoodItemTests
{
    [Fact]
    public void A_food_item_price_cannot_be_negative()
    {
        // Act
        Action create = () => Burger(price: -1m);

        // Assert
        create.Should().Throw<DomainException>();
    }

    [Fact]
    public void A_food_item_cannot_be_updated_to_a_negative_price()
    {
        // Arrange
        var item = Burger();

        // Act
        Action update = () => item.UpdateInfo("Burger", "Beef burger", -5m);

        // Assert
        update.Should().Throw<DomainException>();
    }

    [Fact]
    public void A_food_item_is_available_by_default()
    {
        // Act
        var item = Burger();

        // Assert
        item.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public void Marking_a_food_item_unavailable_makes_it_unorderable()
    {
        // Arrange
        var item = Burger();

        // Act
        item.MakeUnavailable();

        // Assert
        item.IsAvailable.Should().BeFalse();

        // Act
        item.MakeAvailable();

        // Assert
        item.IsAvailable.Should().BeTrue();
    }

    private static FoodItem Burger(decimal price = 50m) =>
        FoodItem.Create(Guid.NewGuid(), "Burger", "Beef burger", price);
}
