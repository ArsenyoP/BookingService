using Booking.Domain.Entities;
using Booking.Domain.Enums;

namespace BookingService.UnitTests.DomainTests.BookingTests
{
    public class Helpers
    {
        public User CreateUser()
        {
            DateOnly birthdayDate = new DateOnly(2, 10, 2000);

            var userResult = User.Create(
                "TestFirstName",
                "TestLastName",
                birthdayDate,
                "testemail@gmail.com",
                "TestUsername");

            return userResult.Value!;
        }

        public Room CreateRoom()
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
    }
}
