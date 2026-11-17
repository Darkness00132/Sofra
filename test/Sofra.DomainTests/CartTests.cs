using Sofra.Domain.Entities;
using Sofra.Domain.Exceptions;

namespace Sofra.DomainTests;

/// <summary>Cart rules — single restaurant per cart, quantity merging, availability (SRS FR-19..FR-22).</summary>
public class CartTests
{
    private static readonly Guid CustomerId = Guid.NewGuid();

    [Fact]
    public void A_cart_can_hold_items_from_a_single_restaurant()
    {
        var cart = Cart.CreateForRestaurant(CustomerId, RestaurantId());
        var item = ItemFor(restaurantId: RestaurantId());

        cart.AddItem(item, quantity: 2);

        Assert.False(cart.IsEmpty);
        Assert.Equal(item.Id, cart.CartItems.Single().FoodItemId);
    }

    [Fact]
    public void Adding_items_from_a_different_restaurant_is_rejected()
    {
        var cart = Cart.CreateForRestaurant(CustomerId, RestaurantId());
        cart.AddItem(ItemFor(RestaurantId()), quantity: 1);

        var otherRestaurantItem = ItemFor(RestaurantId());

        Assert.Throws<DomainException>(() => cart.AddItem(otherRestaurantItem, quantity: 1));
    }

    [Fact]
    public void Adding_the_same_item_again_increases_its_quantity_instead_of_duplicating_it()
    {
        var cart = Cart.CreateForRestaurant(CustomerId, RestaurantId());
        var item = ItemFor(RestaurantId());

        cart.AddItem(item, quantity: 2);
        cart.AddItem(item, quantity: 3);

        var cartItem = Assert.Single(cart.CartItems);
        Assert.Equal(5, cartItem.Quantity);
    }

    [Fact]
    public void An_unavailable_food_item_cannot_be_added_to_the_cart()
    {
        var cart = Cart.CreateForRestaurant(CustomerId, RestaurantId());
        var item = ItemFor(RestaurantId());
        item.MakeUnavailable();

        Assert.Throws<DomainException>(() => cart.AddItem(item, quantity: 1));
    }

    [Fact]
    public void A_cart_item_quantity_must_be_greater_than_zero()
    {
        var cart = Cart.CreateForRestaurant(CustomerId, RestaurantId());

        Assert.Throws<DomainException>(() => cart.AddItem(ItemFor(RestaurantId()), quantity: 0));
    }

    [Fact]
    public void Removing_the_last_item_detaches_the_cart_from_its_restaurant()
    {
        var cart = Cart.CreateForRestaurant(CustomerId, RestaurantId());
        var item = ItemFor(RestaurantId());
        cart.AddItem(item, quantity: 1);

        cart.RemoveItem(item.Id);

        Assert.True(cart.IsEmpty);
        Assert.Null(cart.RestaurantId);
    }

    [Fact]
    public void An_item_that_is_not_in_the_cart_cannot_be_removed()
    {
        var cart = Cart.CreateForRestaurant(CustomerId, RestaurantId());

        Assert.Throws<DomainException>(() => cart.RemoveItem(Guid.NewGuid()));
    }

    private static Guid RestaurantId() => Guid.NewGuid();

    private static FoodItem ItemFor(Guid restaurantId, string name = "Burger", decimal price = 50m) =>
        FoodItem.Create(
            FoodType.Create(restaurantId, Guid.NewGuid(), "Burgers"),
            name, "Beef burger", price);
}
