using Booking.Application.DTOs.Bookings;
using Booking.Application.UseCases.Bookings.CreateBooking;
using Booking.Application.UseCases.Bookings.GetByUserId;
using Booking.Domain.Errors;
using FluentAssertions;

namespace BookingService.IntegrationTests.IntegrationTests.BookingIntegrationTests
{
    public class GetBookingByUserIdIntegrationTests : BaseIntegrationTest
    {
        private readonly Helpers helpers;

        public GetBookingByUserIdIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
            helpers = new Helpers(Sender, DbContext);
        }

        [Fact]
        public async Task GetByUserId_UserAndBookingsExist_ReturnsBookingDtoList()
        {
            var listingId = await helpers.CreateTestListing("101");
            var roomId = await helpers.CreateTestRoom(listingId);
            var userId = await helpers.CreateTestUser();

            var startDate1 = new DateOnly(2030, 12, 1);
            var endDate1 = new DateOnly(2030, 12, 20);

            var startDate2 = new DateOnly(2031, 12, 1);
            var endDate2 = new DateOnly(2031, 12, 20);

            var createBookingDto1 = new CreateBookingDto(roomId, startDate1, endDate1, 1, 2);
            var createBookingDto2 = new CreateBookingDto(roomId, startDate2, endDate2, 1, 2);

            var createBookingCommand1 = new CreateBookingCommand(createBookingDto1, userId);
            var createBookingCommand2 = new CreateBookingCommand(createBookingDto2, userId);

            await Sender.Send(createBookingCommand1);
            await Sender.Send(createBookingCommand2);

            var getByUserIdQuery = new GetByUserIdQuery(userId, 1, 10);

            var result = await Sender.Send(getByUserIdQuery);

            result.IsSuccess.Should().BeTrue();
            result.Value!.Count.Should().Be(2);
            result.Value.All(b => b.GuestId == userId).Should().BeTrue();
        }

        [Fact]
        public async Task GetByUserId_UserExistsButHasNoBookings_ReturnsEmptyList()
        {
            var userId = await helpers.CreateTestUser();

            var getByUserIdQuery = new GetByUserIdQuery(userId, 1, 10);

            var result = await Sender.Send(getByUserIdQuery);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByUserId_UserDoesNotExist_ReturnsFailureNotFound()
        {
            var nonExistentUserId = Guid.NewGuid();

            var getByUserIdQuery = new GetByUserIdQuery(nonExistentUserId, 1, 10);

            var result = await Sender.Send(getByUserIdQuery);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(UserErrors.NotFound);
        }
    }
}
