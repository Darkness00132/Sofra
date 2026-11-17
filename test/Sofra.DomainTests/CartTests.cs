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
        var restaurant = Guid.NewGuid();
        var cart = Cart.CreateForRestaurant(CustomerId, restaurant);
        var item = ItemFor(restaurant);

        cart.AddItem(item, quantity: 2);

        cart.IsEmpty.Should().BeFalse();
        cart.CartItems.Should().ContainSingle().Which.FoodItemId.Should().Be(item.Id);
    }

    [Fact]
    public void Adding_items_from_a_different_restaurant_is_rejected()
    {
        var restaurant = Guid.NewGuid();
        var cart = Cart.CreateForRestaurant(CustomerId, restaurant);
        cart.AddItem(ItemFor(restaurant), quantity: 1);

        var otherRestaurantItem = ItemFor(Guid.NewGuid());

        FluentActions.Invoking(() => cart.AddItem(otherRestaurantItem, quantity: 1))
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void Adding_the_same_item_again_increases_its_quantity_instead_of_duplicating_it()
    {
        var restaurant = Guid.NewGuid();
        var cart = Cart.CreateForRestaurant(CustomerId, restaurant);
        var item = ItemFor(restaurant);

        cart.AddItem(item, quantity: 2);
        cart.AddItem(item, quantity: 3);

        cart.CartItems.Should().ContainSingle().Which.Quantity.Should().Be(5);
    }

    [Fact]
    public void An_unavailable_food_item_cannot_be_added_to_the_cart()
    {
        var restaurant = Guid.NewGuid();
        var cart = Cart.CreateForRestaurant(CustomerId, restaurant);
        var item = ItemFor(restaurant);
        item.MakeUnavailable();

        FluentActions.Invoking(() => cart.AddItem(item, quantity: 1))
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void A_cart_item_quantity_must_be_greater_than_zero()
    {
        var restaurant = Guid.NewGuid();
        var cart = Cart.CreateForRestaurant(CustomerId, restaurant);

        FluentActions.Invoking(() => cart.AddItem(ItemFor(restaurant), quantity: 0))
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void Removing_the_last_item_detaches_the_cart_from_its_restaurant()
    {
        var restaurant = Guid.NewGuid();
        var cart = Cart.CreateForRestaurant(CustomerId, restaurant);
        var item = ItemFor(restaurant);
        cart.AddItem(item, quantity: 1);

        cart.RemoveItem(item.Id);

        cart.IsEmpty.Should().BeTrue();
        cart.RestaurantId.Should().BeNull();
    }

    [Fact]
    public void An_item_that_is_not_in_the_cart_cannot_be_removed()
    {
        var restaurant = Guid.NewGuid();
        var cart = Cart.CreateForRestaurant(CustomerId, restaurant);

        FluentActions.Invoking(() => cart.RemoveItem(Guid.NewGuid()))
            .Should().Throw<DomainException>();
    }

    private static FoodItem ItemFor(Guid restaurantId, string name = "Burger", decimal price = 50m) =>
        FoodItem.Create(
            FoodType.Create(restaurantId, Guid.NewGuid(), "Burgers"),
            name, "Beef burger", price);
}
