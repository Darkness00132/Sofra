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
        var restaurant = RestaurantId();
        var cart = CartFor(restaurant);
        var item = Burger(restaurant);

        cart.AddItem(item, quantity: 2);

        cart.IsEmpty.Should().BeFalse();
        cart.CartItems.Should().ContainSingle().Which.FoodItemId.Should().Be(item.Id);
    }

    [Fact]
    public void Adding_items_from_a_different_restaurant_is_rejected()
    {
        var restaurant = RestaurantId();
        var cart = CartFor(restaurant);
        cart.AddItem(Burger(restaurant), quantity: 1);

        FluentActions.Invoking(() => cart.AddItem(Burger(RestaurantId()), quantity: 1))
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void Adding_the_same_item_again_increases_its_quantity_instead_of_duplicating_it()
    {
        var restaurant = RestaurantId();
        var cart = CartFor(restaurant);
        var item = Burger(restaurant);

        cart.AddItem(item, quantity: 2);
        cart.AddItem(item, quantity: 3);

        cart.CartItems.Should().ContainSingle().Which.Quantity.Should().Be(5);
    }

    [Fact]
    public void An_unavailable_food_item_cannot_be_added_to_the_cart()
    {
        var restaurant = RestaurantId();
        var cart = CartFor(restaurant);
        var item = Burger(restaurant);
        item.MakeUnavailable();

        FluentActions.Invoking(() => cart.AddItem(item, quantity: 1))
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void A_cart_item_quantity_must_be_greater_than_zero()
    {
        var restaurant = RestaurantId();
        var cart = CartFor(restaurant);

        FluentActions.Invoking(() => cart.AddItem(Burger(restaurant), quantity: 0))
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void Removing_the_last_item_detaches_the_cart_from_its_restaurant()
    {
        var restaurant = RestaurantId();
        var cart = CartFor(restaurant);
        var item = Burger(restaurant);
        cart.AddItem(item, quantity: 1);

        cart.RemoveItem(item.Id);

        cart.IsEmpty.Should().BeTrue();
        cart.RestaurantId.Should().BeNull();
    }

    [Fact]
    public void An_item_that_is_not_in_the_cart_cannot_be_removed()
    {
        var restaurant = RestaurantId();
        var cart = CartFor(restaurant);

        FluentActions.Invoking(() => cart.RemoveItem(Guid.NewGuid()))
            .Should().Throw<DomainException>();
    }

    private static Guid RestaurantId() => Guid.NewGuid();

    private static Cart CartFor(Guid restaurantId) => Cart.CreateForRestaurant(CustomerId, restaurantId);

    private static FoodItem Burger(Guid restaurantId, decimal price = 50m) =>
        FoodItem.Create(
            FoodType.Create(restaurantId, Guid.NewGuid(), "Burgers"),
            "Burger", "Beef burger", price);
}
