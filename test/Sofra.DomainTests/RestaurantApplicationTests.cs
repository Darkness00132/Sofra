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
        // Act
        var application = PendingApplication();

        // Assert
        application.Status.Should().Be(RestaurantStatus.PendingApproval);
    }

    [Fact]
    public void Approving_an_application_records_who_reviewed_it_and_when()
    {
        // Arrange
        var application = PendingApplication();
        var reviewerId = Guid.NewGuid();

        // Act
        application.Approve(reviewerId);

        // Assert
        application.Status.Should().Be(RestaurantStatus.Approved);
        application.ReviewedByUserId.Should().Be(reviewerId);
        application.ReviewedAt.Should().NotBeNull();
    }

    [Fact]
    public void Cannot_approve_an_application_that_was_already_approved()
    {
        // Arrange
        var application = ApprovedApplication();

        // Act
        Action approveAgain = () => application.Approve(Guid.NewGuid());

        // Assert
        approveAgain.Should().Throw<DomainException>();
    }

    [Fact]
    public void Cannot_reject_an_application_that_was_already_approved()
    {
        // Arrange
        var application = ApprovedApplication();

        // Act
        Action reject = () => application.Reject("Too many kitchens", Guid.NewGuid());

        // Assert
        reject.Should().Throw<DomainException>();
    }

    [Fact]
    public void Cannot_approve_an_application_that_was_already_rejected()
    {
        // Arrange
        var application = RejectedApplication();

        // Act
        Action approve = () => application.Approve(Guid.NewGuid());

        // Assert
        approve.Should().Throw<DomainException>();
    }

    [Fact]
    public void A_rejection_requires_a_reason()
    {
        // Arrange
        var application = PendingApplication();

        // Act
        Action reject = () => application.Reject("   ", Guid.NewGuid());

        // Assert
        reject.Should().Throw<DomainException>();
    }

    [Fact]
    public void A_restaurant_can_only_be_linked_to_an_approved_application()
    {
        // Arrange
        var application = PendingApplication();

        // Act
        Action link = () => application.LinkRestaurant(Guid.NewGuid());

        // Assert
        link.Should().Throw<DomainException>();
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
