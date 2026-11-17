using Sofra.Domain.Entities;
using Sofra.Domain.Enums;
using Sofra.Domain.Exceptions;

namespace Sofra.DomainTests;

/// <summary>Restaurant approval state machine and order-acceptance rules (SRS BR-01, BR-09).</summary>
public class RestaurantTests
{
    [Fact]
    public void Cannot_be_created_from_an_application_that_is_not_approved()
    {
        var application = RestaurantApplication.Create(
            Guid.NewGuid(), "Sofra Kitchen", "12 Nile St, Cairo", "01000000000", "Family kitchen");

        Assert.Throws<DomainException>(() => Restaurant.FromApprovedApplication(application));
    }

    [Fact]
    public void A_restaurant_created_from_an_approved_application_accepts_orders()
    {
        var restaurant = ApprovedRestaurant();

        Assert.Equal(RestaurantStatus.Approved, restaurant.Status);
        Assert.True(restaurant.IsAcceptingOrders);
        Assert.True(restaurant.CanAcceptNewOrders);
    }

    [Fact]
    public void Suspending_a_restaurant_stops_it_from_accepting_orders()
    {
        var restaurant = ApprovedRestaurant();

        restaurant.Suspend();

        Assert.Equal(RestaurantStatus.Suspended, restaurant.Status);
        Assert.False(restaurant.IsAcceptingOrders);
        Assert.False(restaurant.CanAcceptNewOrders);
    }

    [Fact]
    public void Cannot_suspend_a_restaurant_that_is_already_suspended()
    {
        var restaurant = ApprovedRestaurant();
        restaurant.Suspend();

        Assert.Throws<DomainException>(() => restaurant.Suspend());
    }

    [Fact]
    public void Reinstating_a_suspended_restaurant_restores_order_acceptance()
    {
        var restaurant = ApprovedRestaurant();
        restaurant.Suspend();

        restaurant.Reinstate();

        Assert.Equal(RestaurantStatus.Approved, restaurant.Status);
        Assert.True(restaurant.IsAcceptingOrders);
        Assert.True(restaurant.CanAcceptNewOrders);
    }

    [Fact]
    public void Cannot_reinstate_a_restaurant_that_is_not_suspended()
    {
        var restaurant = ApprovedRestaurant();

        Assert.Throws<DomainException>(() => restaurant.Reinstate());
    }

    [Fact]
    public void Restaurant_name_cannot_be_updated_to_an_empty_value()
    {
        var restaurant = ApprovedRestaurant();

        Assert.Throws<DomainException>(() =>
            restaurant.UpdateInfo("   ", "12 Nile St, Cairo", "01000000000", "Family kitchen"));
    }

    private static Restaurant ApprovedRestaurant()
    {
        var application = RestaurantApplication.Create(
            Guid.NewGuid(), "Sofra Kitchen", "12 Nile St, Cairo", "01000000000", "Family kitchen");
        application.Approve(Guid.NewGuid());

        return Restaurant.FromApprovedApplication(application);
    }
}
