using Booking.Domain.DomainEvents;
using Booking.Domain.Entities;
using FluentAssertions;

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
    }
}
