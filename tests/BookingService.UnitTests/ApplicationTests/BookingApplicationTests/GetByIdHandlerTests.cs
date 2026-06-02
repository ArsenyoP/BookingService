using Booking.Application.DTOs.Bookings;
using Booking.Application.Interfaces.IQueries;
using Booking.Application.UseCases.Bookings.GetById;
using Booking.Domain.Errors;
using FluentAssertions;
using Moq;

namespace BookingService.UnitTests.ApplicationTests.BookingApplicationTests
{
    public class GetByIdHandlerTests
    {
        private readonly Mock<IBookingQueries> _bookingQueriesMock;
        private readonly GetByIdHandler _handler;

        public GetByIdHandlerTests()
        {
            _bookingQueriesMock = new Mock<IBookingQueries>();
            _handler = new GetByIdHandler(_bookingQueriesMock.Object);
        }

        [Fact]
        public async Task Handle_BookingNotFound_ShouldReturnBookingNotFoundFailure()
        {
            var query = new GetByIdQuery(Guid.NewGuid());

            _bookingQueriesMock
                .Setup(x => x.GetByIdAsync(query.id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((BookingResponseDto)null!);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(BookingErrors.NotFound);
        }

        [Fact]
        public async Task Handle_ValidData_ReturnsSuccess()
        {
            var query = new GetByIdQuery(Guid.NewGuid());
            var expected = new BookingResponseDto(
                query.id,
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(2),
                2,
                100,
                200,
                2,
                1,
                "Pending",
                "Test room",
                "Test",
                "User");

            _bookingQueriesMock
                .Setup(x => x.GetByIdAsync(query.id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expected);
            _bookingQueriesMock.Verify(x => x.GetByIdAsync(query.id, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
