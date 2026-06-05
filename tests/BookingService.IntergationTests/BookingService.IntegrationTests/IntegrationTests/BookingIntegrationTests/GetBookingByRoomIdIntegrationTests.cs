using Booking.Application.DTOs.Bookings;
using Booking.Application.UseCases.Bookings.CreateBooking;
using Booking.Application.UseCases.Bookings.GetByRoomId;
using Booking.Application.UseCases.Listing.CreateListing;
using Booking.Application.UseCases.Room.CreateRoom;
using Booking.Domain.Errors;
using FluentAssertions;

namespace BookingService.IntegrationTests.IntegrationTests.BookingIntegrationTests
{
    public class GetBookingByRoomIdIntegrationTests : BaseIntegrationTest
    {
        private readonly Helpers helpers;
        public GetBookingByRoomIdIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
            helpers = new Helpers(Sender, DbContext);
        }

        [Fact]
        public async Task GetByRoomId_AllExists_ReturnsBookingDtoList()
        {
            var createListingCommand = new CreateListingCommand("Listing test title",
                "Listing test description",
                "Ukraine",
                "Ternopil",
                "Saharova",
                "67",
                4,
                Booking.Domain.Enums.ListingType.Apartment);

            var listingId = await Sender.Send(createListingCommand);

            var createRoomCommand = new CreateRoomCommand(
                "TestRoomTitle",
                "Test room description",
                Booking.Domain.Enums.RoomType.ThreeBedFlat,
                250m,
                2,
                2,
                listingId.Value);

            var roomId = await Sender.Send(createRoomCommand);

            var userId = await helpers.CreateTestUser();

            var startDate1 = new DateOnly(2030, 12, 1);
            var endDate1 = new DateOnly(2030, 12, 20);

            var startDate2 = new DateOnly(2031, 12, 1);
            var endDate2 = new DateOnly(2031, 12, 20);

            var createBookingDto1 = new CreateBookingDto(roomId.Value,
                startDate1,
                endDate1,
                1,
                2);

            var createBookingDto2 = new CreateBookingDto(roomId.Value,
                startDate2,
                endDate2,
                1,
                2);

            var createBookingCommand1 = new CreateBookingCommand(createBookingDto1, userId);
            var createBookingCommand2 = new CreateBookingCommand(createBookingDto2, userId);

            await Sender.Send(createBookingCommand1);
            await Sender.Send(createBookingCommand2);

            var getByRoomIdQuery = new GetByRoomIdQuery(roomId.Value, 1, 10);


            var result = await Sender.Send(getByRoomIdQuery);


            result.IsSuccess.Should().BeTrue();
            result.Value!.Count.Should().Be(2);
            result.Value.All(b => b.RoomId == roomId.Value).Should().BeTrue();
        }
        [Fact]
        public async Task GetByRoomId_RoomExistsButHasNoBookings_ReturnsEmptyList()
        {
            var createListingCommand = new CreateListingCommand("Listing test title",
                "Listing test description",
                "Ukraine",
                "Ternopil",
                "Saharova",
                "68",
                4,
                Booking.Domain.Enums.ListingType.Apartment);

            var listingId = await Sender.Send(createListingCommand);

            var createRoomCommand = new CreateRoomCommand(
                "TestRoomTitle",
                "Test room description",
                Booking.Domain.Enums.RoomType.ThreeBedFlat,
                250m,
                2,
                2,
                listingId.Value);

            var roomId = await Sender.Send(createRoomCommand);

            var getByRoomIdQuery = new GetByRoomIdQuery(roomId.Value, 1, 10);

            var result = await Sender.Send(getByRoomIdQuery);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByRoomId_RoomDoesNotExist_ReturnsFailureNotFound()
        {
            var nonExistentRoomId = Guid.NewGuid();

            var getByRoomIdQuery = new GetByRoomIdQuery(nonExistentRoomId, 1, 10);

            var result = await Sender.Send(getByRoomIdQuery);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(RoomErrors.NotFound);
        }
    }
}
