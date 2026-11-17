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
        var application = PendingApplication();

        FluentActions.Invoking(() => Restaurant.FromApprovedApplication(application))
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void A_restaurant_created_from_an_approved_application_accepts_orders()
    {
        var restaurant = ApprovedRestaurant();

        restaurant.Status.Should().Be(RestaurantStatus.Approved);
        restaurant.IsAcceptingOrders.Should().BeTrue();
        restaurant.CanAcceptNewOrders.Should().BeTrue();
    }

    [Fact]
    public void Suspending_a_restaurant_stops_it_from_accepting_orders()
    {
        var restaurant = ApprovedRestaurant();

        restaurant.Suspend();

        restaurant.Status.Should().Be(RestaurantStatus.Suspended);
        restaurant.IsAcceptingOrders.Should().BeFalse();
        restaurant.CanAcceptNewOrders.Should().BeFalse();
    }

    [Fact]
    public void Cannot_suspend_a_restaurant_that_is_already_suspended()
    {
        var restaurant = ApprovedRestaurant();
        restaurant.Suspend();

        FluentActions.Invoking(() => restaurant.Suspend())
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void Reinstating_a_suspended_restaurant_restores_order_acceptance()
    {
        var restaurant = ApprovedRestaurant();
        restaurant.Suspend();

        restaurant.Reinstate();

        restaurant.Status.Should().Be(RestaurantStatus.Approved);
        restaurant.IsAcceptingOrders.Should().BeTrue();
        restaurant.CanAcceptNewOrders.Should().BeTrue();
    }

    [Fact]
    public void Cannot_reinstate_a_restaurant_that_is_not_suspended()
    {
        var restaurant = ApprovedRestaurant();

        FluentActions.Invoking(() => restaurant.Reinstate())
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void Restaurant_name_cannot_be_updated_to_an_empty_value()
    {
        var restaurant = ApprovedRestaurant();

        FluentActions.Invoking(() => restaurant.UpdateInfo("   ", Address, Phone, Description))
            .Should().Throw<DomainException>();
    }

    private const string Address = "12 Nile St, Cairo";
    private const string Phone = "01000000000";
    private const string Description = "Family kitchen";

    private static RestaurantApplication PendingApplication() => RestaurantApplication.Create(
        Guid.NewGuid(), "Sofra Kitchen", Address, Phone, Description);

    private static Restaurant ApprovedRestaurant()
    {
        var application = PendingApplication();
        application.Approve(Guid.NewGuid());

        return Restaurant.FromApprovedApplication(application);
    }
}
