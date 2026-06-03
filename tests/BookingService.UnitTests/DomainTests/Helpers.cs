using Booking.Domain.Entities;
using Booking.Domain.Enums;
using Booking.Domain.ValueObjects;

namespace BookingService.UnitTests.DomainTests
{
    internal static class Helpers
    {
        public static User CreateTestUser(Guid? userId = null)
        {
            DateOnly birthdayDate = new DateOnly(2000, 10, 10);

            Guid finalUserId = userId ?? Guid.Parse("c23a9673-90a1-42a5-a6f4-dd7ed7055496");

            var userResult = User.Create(
                "TestFirstName",
                "TestLastName",
                birthdayDate,
                "testemail@gmail.com",
                "TestUsername");

            userResult.Value.Id = finalUserId;
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

        public static Bookings CreateTestBooking(Guid? userId = null)
        {
            Guid finalUserId = userId ?? Guid.Parse("c23a9673-90a1-42a5-a6f4-dd7ed7055496");

            var room = CreateTestRoom();
            var user = CreateTestUser(finalUserId);
            var dateRange = CreateTestDateRange();

            return Bookings.Create(dateRange, 2, 1, room, user).Value!;
        }
        public static Address CreateTestAddress()
        {
            return Address.Create("Ukraine", "Ternopil", "Main Street", "10", 9).Value!;
        }

        public static Listing CreateTestListing()
        {
            var address = Address.Create("Ukraine", "Ternopil", "Main Street", "10", 9).Value!;
            return Listing.Create("Cozy Apartment", "Nice place to stay", address, ListingType.Apartment).Value!;
        }

        public static Amenity CreateTestAmenity(string name)
        {
            return Amenity.Create(name, AmenityCategory.Entertainment).Value!;
        }

        public static Review CreateTestReview(int score = 5, string text = "Excellent service and comfortable room.")
        {
            return Review.Create(Guid.NewGuid(), Guid.NewGuid(), ReviewsTargetType.Room, score, text).Value!;
        }
    }
}
