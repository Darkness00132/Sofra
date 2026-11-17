using Sofra.Domain.Entities;
using Sofra.Domain.Enums;
using Sofra.Domain.Exceptions;

namespace Sofra.DomainTests;

/// <summary>Application review rules — an application is decided exactly once (SRS FR-47/FR-48).</summary>
public class RestaurantApplicationTests
{
    private static RestaurantApplication PendingApplication() => RestaurantApplication.Create(
        Guid.NewGuid(), "Sofra Kitchen", "12 Nile St, Cairo", "01000000000", "Family kitchen");

    [Fact]
    public void A_new_application_is_pending_approval()
    {
        var application = PendingApplication();

        Assert.Equal(RestaurantStatus.PendingApproval, application.Status);
    }

    [Fact]
    public void Approving_an_application_records_who_reviewed_it_and_when()
    {
        var application = PendingApplication();
        var reviewerId = Guid.NewGuid();

        application.Approve(reviewerId);

        Assert.Equal(RestaurantStatus.Approved, application.Status);
        Assert.Equal(reviewerId, application.ReviewedByUserId);
        Assert.NotNull(application.ReviewedAt);
    }

    [Fact]
    public void Cannot_approve_an_application_that_was_already_approved()
    {
        var application = PendingApplication();
        application.Approve(Guid.NewGuid());

        Assert.Throws<DomainException>(() => application.Approve(Guid.NewGuid()));
    }

    [Fact]
    public void Cannot_reject_an_application_that_was_already_approved()
    {
        var application = PendingApplication();
        application.Approve(Guid.NewGuid());

        Assert.Throws<DomainException>(() => application.Reject("Too many kitchens", Guid.NewGuid()));
    }

    [Fact]
    public void Cannot_approve_an_application_that_was_already_rejected()
    {
        var application = PendingApplication();
        application.Reject("Incomplete documents", Guid.NewGuid());

        Assert.Throws<DomainException>(() => application.Approve(Guid.NewGuid()));
    }

    [Fact]
    public void A_rejection_requires_a_reason()
    {
        var application = PendingApplication();

        Assert.Throws<DomainException>(() => application.Reject("   ", Guid.NewGuid()));
    }

    [Fact]
    public void A_restaurant_can_only_be_linked_to_an_approved_application()
    {
        var application = PendingApplication();

        Assert.Throws<DomainException>(() => application.LinkRestaurant(Guid.NewGuid()));
    }
}
