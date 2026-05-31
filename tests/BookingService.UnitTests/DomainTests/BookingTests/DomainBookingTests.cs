using Booking.Domain.DomainEvents;
using Booking.Domain.Entities;
using Booking.Domain.Interfaces;
using Booking.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace BookingService.UnitTests.DomainTests.BookingTests
{
    public class DomainBookingTests
    {
        [Fact]
        public void Create_ValidParams_IsSuccessTrue()
        {
            var room = Helpers.CreateTestRoom();
            var user = Helpers.CreateTestUser();
            var dateRange = Helpers.CreateTestDateRange();

            var result = Bookings.Create(dateRange, 2, 2, room, user);

            result.IsSuccess.Should().Be(true);
        }

        [Fact]
        public void Create_NegativeAdults_IsSuccessFalse()
        {
            var room = Helpers.CreateTestRoom();
            var user = Helpers.CreateTestUser();
            var dateRange = Helpers.CreateTestDateRange();

            var result = Bookings.Create(dateRange, -1, 2, room, user);

            result.IsSuccess.Should().Be(false);
            result.Error.Code.Should().Be("Booking.AtLeastOneAdultRequired");
            result.Value.Should().BeNull();
        }
        [Fact]
        public void Create_NegativeChildren_IsSuccessFalse()
        {
            var room = Helpers.CreateTestRoom();
            var user = Helpers.CreateTestUser();
            var dateRange = Helpers.CreateTestDateRange();

            var result = Bookings.Create(dateRange, 2, -1, room, user);

            result.IsSuccess.Should().Be(false);
            result.Error.Code.Should().Be("Booking.NegativeChildrenCount");
            result.Value.Should().BeNull();
        }

        [Fact]
        public void Create_GuestsExceedCapacity_IsSuccessFalse()
        {
            var room = Helpers.CreateTestRoom();
            var user = Helpers.CreateTestUser();
            var dateRange = Helpers.CreateTestDateRange();

            var result = Bookings.Create(dateRange, 4, 2, room, user);

            result.IsSuccess.Should().Be(false);
            result.Error.Code.Should().Be("BookingErrors.ExceedsCapacity");
            result.Value.Should().BeNull();
        }

        [Fact]
        public void Create_ZeroAdults_IsSuccessFalse()
        {
            var room = Helpers.CreateTestRoom();
            var user = Helpers.CreateTestUser();
            var dateRange = Helpers.CreateTestDateRange();

            var result = Bookings.Create(dateRange, 0, 2, room, user);

            result.IsSuccess.Should().Be(false);
            result.Error.Code.Should().Be("Booking.AtLeastOneAdultRequired");
            result.Value.Should().BeNull();
        }

        [Fact]
        public void Create_ZeroChildren_IsSuccessTrue()
        {
            var room = Helpers.CreateTestRoom();
            var user = Helpers.CreateTestUser();
            var dateRange = Helpers.CreateTestDateRange();

            var result = Bookings.Create(dateRange, 2, 0, room, user);

            result.IsSuccess.Should().Be(true);
            result.Value.Should().NotBeNull();
            result.Value!.ChildrenCount.Should().Be(0);
        }

        [Fact]
        public void Create_ExactMaxCapacity_IsSuccessTrue()
        {
            var room = Helpers.CreateTestRoom(); // 3 дорослих + 2 дітей = 5
            var user = Helpers.CreateTestUser();
            var dateRange = Helpers.CreateTestDateRange();

            var result = Bookings.Create(dateRange, 3, 2, room, user);

            result.IsSuccess.Should().Be(true);
            result.Value.Should().NotBeNull();
            result.Value!.AdultsCount.Should().Be(3);
            result.Value!.ChildrenCount.Should().Be(2);
        }


        [Fact]
        public void Create_ValidParams_ShouldRaiseBookingCreatedDomainEvent()
        {
            var room = Helpers.CreateTestRoom();
            var user = Helpers.CreateTestUser();
            var dateRange = Helpers.CreateTestDateRange();

            var result = Bookings.Create(dateRange, 2, 1, room, user);

            result.IsSuccess.Should().Be(true);
            result.Value.Should().NotBeNull();

            var events = result.Value.GetDomainEvents().ToList();

            events.Should().ContainSingle(x => x is BookingCreatedDomainEvent);
        }

        [Fact]
        public void Confirm_ValidData_StatusConfirmed()
        {
            var booking = Helpers.CreateTestBooking();

            booking.Confirm();

            booking.Status.Should().Be(Booking.Domain.Enums.BookingStatus.Confirmed);
        }

        [Fact]
        public void Cancel_ValidData_ShouldReturnSuccessAndSetStatusToCancelled()
        {
            var booking = Helpers.CreateTestBooking();
            var utcNow = DateTime.UtcNow;
            var refundValue = new RefundValue(
                PercentToRefund: 100,
                TotalBookingPrice: 2000
            );

            var refundPolicyMock = new Mock<IRefundPolicy>();
            refundPolicyMock.Setup(x => x.CalculateRefund(booking, utcNow))
                .Returns(refundValue);

            var result = booking.Cancel(utcNow, refundPolicyMock.Object);

            booking.ConfirmationToken.Should().BeNull();
            booking.Status.Should().Be(Booking.Domain.Enums.BookingStatus.Cancelled);
        }

        [Fact]
        public void Cancel_NowInFuture_ShouldReturnIsSuccessFalseAndCodeCannotCancelStartedBooking()
        {
            var booking = Helpers.CreateTestBooking();
            var utcNow = DateTime.UtcNow.AddYears(100);
            var refundValue = new RefundValue(
                PercentToRefund: 100,
                TotalBookingPrice: 2000
            );

            var refundPolicyMock = new Mock<IRefundPolicy>();
            refundPolicyMock.Setup(x => x.CalculateRefund(booking, utcNow))
                .Returns(refundValue);

            var result = booking.Cancel(utcNow, refundPolicyMock.Object);

            result.IsSuccess.Should().Be(false);
            result.Error.Code.Should().Be("Booking.CannotCancelStartedBooking");
            booking.ConfirmationToken.Should().NotBeNull();
            result.Value.Should().BeNull();
        }

        [Fact]
        public void Cancel_InvalidStatus_ShouldReturnIsSuccessFalseAndCodeCannotCancel()
        {
            var booking = Helpers.CreateTestBooking();
            var utcNow = DateTime.UtcNow;
            var refundValue = new RefundValue(
                PercentToRefund: 100,
                TotalBookingPrice: 2000
            );

            booking.Completed();

            var refundPolicyMock = new Mock<IRefundPolicy>();
            refundPolicyMock.Setup(x => x.CalculateRefund(booking, utcNow))
                .Returns(refundValue);

            var result = booking.Cancel(utcNow, refundPolicyMock.Object);

            result.IsSuccess.Should().Be(false);
            result.Error.Code.Should().Be("Booking.CannotCancel");
            booking.ConfirmationToken.Should().BeNull();
            result.Value.Should().BeNull();
        }
    }
}
