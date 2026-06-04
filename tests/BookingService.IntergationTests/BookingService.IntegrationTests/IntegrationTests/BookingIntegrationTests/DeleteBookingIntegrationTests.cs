using Booking.Application.UseCases.Booking.DeleteBooking;
using Booking.Domain.Errors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BookingService.IntegrationTests.IntegrationTests.BookingIntegrationTests
{
    public class DeleteBookingIntegrationTests : BaseIntegrationTest
    {
        private readonly Helpers _helpers;
        private readonly IntegrationTestWebAppFactory _factory;
        public DeleteBookingIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _helpers = new Helpers(Sender, DbContext);
        }

        [Fact]
        public async Task Delete_ValidData_ShouldReturnSuccess()
        {
            var createBookingResult = await _helpers.CreateTestBooking();
            createBookingResult.IsSuccess.Should().BeTrue();
            var bookingId = createBookingResult.Value;

            var commandToDelete = new DeleteBookingCommand(bookingId);

            var deleteResult = await Sender.Send(commandToDelete);

            deleteResult.IsSuccess.Should().BeTrue();
            deleteResult.Value.Should().Be(bookingId);

            var bookingInDb = await DbContext.Bookings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == bookingId);

            bookingInDb.Should().BeNull();
        }

        [Fact]
        public async Task Delete_ShouldReturnFailure_WhenBookingDoesNotExist()
        {
            var nonExistentBookingId = Guid.NewGuid();
            var commandToDelete = new DeleteBookingCommand(nonExistentBookingId);

            var result = await Sender.Send(commandToDelete);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(BookingErrors.NotFound);
        }

        [Fact]
        public async Task Delete_Booking_ShouldNotCascadeDeleteRoomOrUser()
        {
            var listingId = await _helpers.CreateTestListing("5");
            var roomId = await _helpers.CreateTestRoom(listingId);
            var userId = await _helpers.CreateTestUser();

            var createBookingResult = await _helpers.CreateTestBooking(roomId);
            var bookingId = createBookingResult.Value;

            var commandToDelete = new DeleteBookingCommand(bookingId);

            var deleteResult = await Sender.Send(commandToDelete);
            deleteResult.IsSuccess.Should().BeTrue();

            var roomInDb = await DbContext.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == roomId);
            roomInDb.Should().NotBeNull();

            var userInDb = await DbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId);
            userInDb.Should().NotBeNull();
        }
    }
}
