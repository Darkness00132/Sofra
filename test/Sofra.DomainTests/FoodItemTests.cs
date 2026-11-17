using Sofra.Domain.Entities;
using Sofra.Domain.Exceptions;

namespace Sofra.DomainTests;

/// <summary>Menu item invariants and availability (SRS FR-15, FR-16, BR-05).</summary>
public class FoodItemTests
{
    [Fact]
    public void A_food_item_price_cannot_be_negative()
    {
        Assert.Throws<DomainException>(() =>
            FoodItem.Create(Guid.NewGuid(), "Burger", "Beef burger", price: -1m));
    }

    [Fact]
    public void A_food_item_cannot_be_updated_to_a_negative_price()
    {
        var item = FoodItem.Create(Guid.NewGuid(), "Burger", "Beef burger", 50m);

        Assert.Throws<DomainException>(() => item.UpdateInfo("Burger", "Beef burger", -5m));
    }

    [Fact]
    public void A_food_item_is_available_by_default()
    {
        var item = FoodItem.Create(Guid.NewGuid(), "Burger", "Beef burger", 50m);

        Assert.True(item.IsAvailable);
    }

    [Fact]
    public void Marking_a_food_item_unavailable_makes_it_unorderable()
    {
        var item = FoodItem.Create(Guid.NewGuid(), "Burger", "Beef burger", 50m);

        item.MakeUnavailable();

        Assert.False(item.IsAvailable);

        item.MakeAvailable();

        Assert.True(item.IsAvailable);
    }
}
