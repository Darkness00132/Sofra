using Sofra.Domain.Entities;
using Sofra.Domain.Enums;
using Sofra.Domain.Exceptions;

namespace Sofra.DomainTests;

/// <summary>Application review rules — an application is decided exactly once (SRS FR-47/FR-48).</summary>
public class RestaurantApplicationTests
{
    [Fact]
    public void A_new_application_is_pending_approval()
    {
        var application = PendingApplication();

        application.Status.Should().Be(RestaurantStatus.PendingApproval);
    }

    [Fact]
    public void Approving_an_application_records_who_reviewed_it_and_when()
    {
        var application = PendingApplication();
        var reviewerId = Guid.NewGuid();

        application.Approve(reviewerId);

        application.Status.Should().Be(RestaurantStatus.Approved);
        application.ReviewedByUserId.Should().Be(reviewerId);
        application.ReviewedAt.Should().NotBeNull();
    }

    [Fact]
    public void Cannot_approve_an_application_that_was_already_approved()
    {
        var application = ApprovedApplication();

        FluentActions.Invoking(() => application.Approve(Guid.NewGuid()))
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void Cannot_reject_an_application_that_was_already_approved()
    {
        var application = ApprovedApplication();

        FluentActions.Invoking(() => application.Reject("Too many kitchens", Guid.NewGuid()))
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void Cannot_approve_an_application_that_was_already_rejected()
    {
        var application = RejectedApplication();

        FluentActions.Invoking(() => application.Approve(Guid.NewGuid()))
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void A_rejection_requires_a_reason()
    {
        var application = PendingApplication();

        FluentActions.Invoking(() => application.Reject("   ", Guid.NewGuid()))
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void A_restaurant_can_only_be_linked_to_an_approved_application()
    {
        var application = PendingApplication();

        FluentActions.Invoking(() => application.LinkRestaurant(Guid.NewGuid()))
            .Should().Throw<DomainException>();
    }

    private static RestaurantApplication PendingApplication() => RestaurantApplication.Create(
        Guid.NewGuid(), "Sofra Kitchen", "12 Nile St, Cairo", "01000000000", "Family kitchen");

    private static RestaurantApplication ApprovedApplication()
    {
        var application = PendingApplication();
        application.Approve(Guid.NewGuid());

        return application;
    }

    private static RestaurantApplication RejectedApplication()
    {
        var application = PendingApplication();
        application.Reject("Incomplete documents", Guid.NewGuid());

        return application;
    }
}
