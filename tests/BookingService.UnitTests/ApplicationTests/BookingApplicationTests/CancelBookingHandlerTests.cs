using Booking.Application.Interfaces;
using Booking.Application.UseCases.Bookings.CancelBooking;
using Booking.Domain.Entities;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces;
using Booking.Domain.Interfaces.IRepositories;
using BookingService.UnitTests.DomainTests;
using FluentAssertions;
using Moq;

namespace BookingService.UnitTests.ApplicationTests.BookingApplicationTests
{
    public class CancelBookingHandlerTests
    {
        private readonly Mock<IBookingRepository> _bookingRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRefundPolicy> _refundPolicyMock;
        private readonly CancelBookingHandler _handler;

        public CancelBookingHandlerTests()
        {
            _bookingRepositoryMock = new Mock<IBookingRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _refundPolicyMock = new Mock<IRefundPolicy>();

            _handler = new CancelBookingHandler(
                _bookingRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _refundPolicyMock.Object
            );
        }

        [Fact]
        public async Task Handle_BookingNotFound_ShouldReturnBookingNotFoundFailure()
        {
            var command = new CancelBookingCommand(Guid.NewGuid(), Guid.NewGuid().ToString());

            _bookingRepositoryMock
                .Setup(x => x.GetById(command.bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Bookings)null!);

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().Be(false);
            result.Error.Should().Be(BookingErrors.NotFound);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UserIsNotGuest_ShouldReturnCannotCancelFailure()
        {
            var bookingGuestId = Guid.NewGuid();
            var differentUserId = Guid.NewGuid().ToString();
            var booking = CreateTestBooking(bookingGuestId);
            var command = new CancelBookingCommand(booking.Id, differentUserId);

            _bookingRepositoryMock
                .Setup(x => x.GetById(command.bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().Be(false);
            result.Error.Should().Be(BookingErrors.CannotCancel);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidData_ReturnsSuccess()
        {
            var guestId = Guid.NewGuid();
            var booking = CreateTestBooking(guestId);
            var command = new CancelBookingCommand(booking.Id, guestId.ToString());

            _bookingRepositoryMock
                .Setup(x => x.GetById(command.bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().Be(true);
            result.Value.Should().Be(booking.Id);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private Bookings CreateTestBooking(Guid guestId)
        {
            return Helpers.CreateTestBooking(guestId);
        }
    }
}
