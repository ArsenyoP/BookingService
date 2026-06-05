using Booking.Application.UseCases.Listing.CreateListing;
using Booking.Application.UseCases.Room.CreateRoom;
using Booking.Application.UseCases.Room.GetByListingId;
using Booking.Domain.Errors;
using FluentAssertions;

namespace BookingService.IntegrationTests.IntegrationTests.RoomIntegrationTests
{
    public class GetRoomByListingIdIntegrationTests : BaseIntegrationTest
    {
        private readonly Helpers helpers;

        public GetRoomByListingIdIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
            helpers = new Helpers(Sender, DbContext);
        }

        [Fact]
        public async Task GetByListingId_ListingAndRoomsExist_ReturnsRoomDtoList()
        {
            var createListingCommand = new CreateListingCommand(
                "Listing test title",
                "Listing test description",
                "Ukraine",
                "Ternopil",
                "Saharova",
                "150",
                4,
                Booking.Domain.Enums.ListingType.Apartment);
            var listingId = await Sender.Send(createListingCommand);

            var createRoomCommand1 = new CreateRoomCommand(
                "Room 1",
                "Description 1",
                Booking.Domain.Enums.RoomType.ThreeBedFlat,
                200m, 2, 2, listingId.Value);

            var createRoomCommand2 = new CreateRoomCommand(
                "Room 2",
                "Description 2",
                Booking.Domain.Enums.RoomType.ThreeBedFlat,
                300m, 3, 3, listingId.Value);

            await Sender.Send(createRoomCommand1);
            await Sender.Send(createRoomCommand2);

            var query = new GetByListingIdQuery(1, 10, listingId.Value);

            var result = await Sender.Send(query);

            result.IsSuccess.Should().BeTrue();
            result.Value!.Count.Should().Be(2);
            result.Value.All(r => r.ListingId == listingId.Value).Should().BeTrue();
        }

        [Fact]
        public async Task GetByListingId_ListingExistsButHasNoRooms_ReturnsEmptyList()
        {
            var createListingCommand = new CreateListingCommand(
                "Listing test title",
                "Listing test description",
                "Ukraine",
                "Ternopil",
                "Saharova",
                "151",
                4,
                Booking.Domain.Enums.ListingType.Apartment);

            var listingId = await Sender.Send(createListingCommand);

            var query = new GetByListingIdQuery(10, 1, listingId.Value);

            var result = await Sender.Send(query);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByListingId_ListingDoesNotExist_ReturnsFailureNotFound()
        {
            var nonExistentListingId = Guid.NewGuid();
            var query = new GetByListingIdQuery(10, 1, nonExistentListingId);

            var result = await Sender.Send(query);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(ListingErrors.NotFound);
        }
    }
}
