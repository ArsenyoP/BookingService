using Booking.Application.DTOs.Bookings;
using Booking.Application.DTOs.Users;
using Booking.Application.UseCases.Bookings.CreateBooking;
using Booking.Application.UseCases.Listing.CreateListing;
using Booking.Application.UseCases.Room.CreateRoom;
using Booking.Application.UseCases.Users.RegisterUser;
using Booking.Domain.Common;
using Booking.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingService.IntegrationTests
{
    public class Helpers(ISender Sender, AppDbContext DbContext)
    {
        public async Task<Guid> CreateTestListing(string HouseNumber = "1")
        {
            var createListingCommand = new CreateListingCommand("Listing test title",
                "Listing test description",
                "Ukraine",
                "Ternopil",
                "Saharova",
                HouseNumber,
                4,
                Booking.Domain.Enums.ListingType.Apartment);

            var listingResult = await Sender.Send(createListingCommand);
            return listingResult.Value;
        }

        public async Task<Guid> CreateTestRoom(Guid listingId)
        {
            var createRoomCommand = new CreateRoomCommand(
                "TestRoomTitle",
                "Test room description",
                Booking.Domain.Enums.RoomType.ThreeBedFlat,
                250m,
                2,
                2,
                listingId);

            var roomResult = await Sender.Send(createRoomCommand);
            return roomResult.Value;
        }

        public async Task<Guid> CreateTestUser()
        {
            var birthdayDate = new DateOnly(2000, 12, 1);
            var registerDto = new RegisterDto(
                "Arsenyo",
                "Arsen",
                "Prysiazhnyi",
                "arsenyo198510@gmail.com",
                "Password123!",
                birthdayDate);
            var createUserCommand = new RegisterUserCommand(registerDto, "Admin");

            var userResult = await Sender.Send(createUserCommand);

            var user = await DbContext.Users.Where(x => x.UserName == registerDto.UserName).FirstOrDefaultAsync();

            return user!.Id;
        }

        public async Task<Result<Guid>> CreateTestBooking(Guid? roomIdInput = null)
        {
            var listingId = await CreateTestListing();
            if (roomIdInput is null)
            {
                roomIdInput = await CreateTestRoom(listingId);
            }

            Guid roomId = (Guid)roomIdInput;

            var userId = await CreateTestUser();

            var startDate = new DateOnly(2030, 12, 1);
            var endDate = new DateOnly(2030, 12, 20);

            var createBookingDto = new CreateBookingDto(roomId,
                startDate,
                endDate,
                1,
                2);

            var createBookingCommand = new CreateBookingCommand(createBookingDto, userId);


            var result = await Sender.Send(createBookingCommand);
            return result;
        }
    }
}
