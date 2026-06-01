using Booking.Application.Interfaces;
using Booking.Application.UseCases.Bookings.ConfirmBooking;
using Booking.Domain.Entities;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;
using BookingService.UnitTests.DomainTests;
using FluentAssertions;
using Moq;

namespace BookingService.UnitTests.ApplicationTests.BookingApplicationTests
{
    public class ConfirmBookingHandlerTests
    {
        private readonly Mock<IBookingRepository> _bookingRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ConfirmBookingHandler _handler;

        public ConfirmBookingHandlerTests()
        {
            _bookingRepositoryMock = new Mock<IBookingRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new ConfirmBookingHandler(
                _bookingRepositoryMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_BookingNotFound_ShouldReturnBookingNotFoundFailure()
        {
            var command = new ConfirmBookingCommand("some-invalid-token");

            _bookingRepositoryMock
                .Setup(x => x.GetBookingEntityByConfirmationToken(command.confirmationToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Bookings)null!);

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().Be(false);
            result.Error.Should().Be(BookingErrors.NotFound);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidData_ReturnsSuccess()
        {
            var booking = Helpers.CreateTestBooking();
            var command = new ConfirmBookingCommand("valid-confirmation-token");

            _bookingRepositoryMock
                .Setup(x => x.GetBookingEntityByConfirmationToken(command.confirmationToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().Be(true);
            result.Value.Should().Be(booking.Id);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
