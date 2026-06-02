using Booking.Application.DTOs.Bookings;
using Booking.Application.UseCases.Bookings.GetAllBookings;
using Booking.Application.Interfaces.IQueries;
using FluentAssertions;
using Moq;

namespace BookingService.UnitTests.ApplicationTests.BookingApplicationTests
{
    public class GetAllBookingsHandlerTests
    {
        private readonly Mock<IBookingQueries> _bookingQueriesMock;
        private readonly GetAllBookingsHandler _handler;

        public GetAllBookingsHandlerTests()
        {
            _bookingQueriesMock = new Mock<IBookingQueries>();
            _handler = new GetAllBookingsHandler(_bookingQueriesMock.Object);
        }

        [Fact]
        public async Task Handle_ValidData_ReturnsSuccess()
        {
            var query = new GetAllBookingsQuery(2, 25);
            var expected = new List<BookingResponseDto>
            {
                new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow.AddDays(2), 2, 100, 200, 2, 1, "Pending", "Test room", "Test", "User")
            };

            _bookingQueriesMock
                .Setup(x => x.GetAllPagedAsync(query.Page, query.PageSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expected);
            _bookingQueriesMock.Verify(x => x.GetAllPagedAsync(query.Page, query.PageSize, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidPagination_ShouldUseDefaultValues()
        {
            var query = new GetAllBookingsQuery(0, 0);
            var expected = new List<BookingResponseDto>();

            _bookingQueriesMock
                .Setup(x => x.GetAllPagedAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expected);
            _bookingQueriesMock.Verify(x => x.GetAllPagedAsync(1, 10, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
