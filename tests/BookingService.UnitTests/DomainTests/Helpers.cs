using Booking.Domain.Entities;
using Booking.Domain.Enums;
using Booking.Domain.ValueObjects;

namespace BookingService.UnitTests.DomainTests
{
    public static class Helpers
    {
        public static User CreateTestUser()
        {
            DateOnly birthdayDate = new DateOnly(2000, 10, 10);

            var userResult = User.Create(
                "TestFirstName",
                "TestLastName",
                birthdayDate,
                "testemail@gmail.com",
                "TestUsername");

            return userResult.Value!;
        }

        public static Room CreateTestRoom()
        {
            Guid listingId = Guid.Parse("c0de5ddc-6db8-4fcd-a2ff-f60dc446d1ed");

            var roomResult = Room.Create(
                "TestRoomTitle",
                "Test room description",
                RoomType.Standard,
                300,
                3,
                2,
                listingId);

            return roomResult.Value!;
        }

        public static DateRange CreateTestDateRange()
        {
            DateOnly startDate = new DateOnly(2026, 10, 10);
            DateOnly endDate = new DateOnly(2026, 10, 12);

            var result = DateRange.Create(startDate, endDate).Value!;
            return result;
        }
    }
}
