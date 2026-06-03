using Booking.Application.DTOs.Bookings;
using Booking.Application.Interfaces;
using Booking.Application.UseCases.Bookings.CreateBooking;
using Booking.Domain.Common;
using Booking.Domain.Entities;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;
using BookingService.UnitTests.DomainTests;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace BookingService.UnitTests.ApplicationTests.BookingApplicationTests
{
    public class CreateBookingHandlerTests
    {
        private readonly Mock<IBookingRepository> _bookingRepositoryMock;
        private readonly Mock<IRoomRepository> _roomRepositoryMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateBookingHandler _handler;

        public CreateBookingHandlerTests()
        {
            _bookingRepositoryMock = new Mock<IBookingRepository>();
            _roomRepositoryMock = new Mock<IRoomRepository>();
            _userManagerMock = CreateUserManagerMock();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _unitOfWorkMock
                .Setup(x => x.ExecuteInSerializableTransactionAsync(
                    It.IsAny<Func<Task<Result<Guid>>>>(),
                    It.IsAny<CancellationToken>()))
                .Returns((Func<Task<Result<Guid>>> operation, CancellationToken ct) => operation());

            _handler = new CreateBookingHandler(
                _bookingRepositoryMock.Object,
                _roomRepositoryMock.Object,
                _userManagerMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_InvalidDateRange_ShouldReturnDateRangeFailure()
        {
            var command = CreateCommand(startDate: new DateOnly(2026, 10, 12), endDate: new DateOnly(2026, 10, 10));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Booking.InvalidEnd");
            _roomRepositoryMock.Verify(x => x.GetByIdWithAmenities(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            _userManagerMock.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Never);
            _bookingRepositoryMock.Verify(x => x.IsRoomAvailableAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_RoomNotFound_ShouldReturnRoomNotFoundFailure()
        {
            var command = CreateCommand();

            _roomRepositoryMock
                .Setup(x => x.GetByIdWithAmenities(command.CreateDto.RoomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Room)null!);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(RoomErrors.NotFound);
            _userManagerMock.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Never);
            _bookingRepositoryMock.Verify(x => x.IsRoomAvailableAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_GuestNotFound_ShouldReturnUserNotFoundFailure()
        {
            var command = CreateCommand();
            var room = Helpers.CreateTestRoom();

            _roomRepositoryMock
                .Setup(x => x.GetByIdWithAmenities(command.CreateDto.RoomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(room);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(command.GuestId.ToString()))
                .ReturnsAsync((User)null!);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(UserErrors.NotFound);
            _bookingRepositoryMock.Verify(x => x.IsRoomAvailableAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()), Times.Never);
            _bookingRepositoryMock.Verify(x => x.Add(It.IsAny<Bookings>()), Times.Never);
        }

        [Fact]
        public async Task Handle_RoomIsNotAvailable_ShouldReturnRoomNotAvailableFailure()
        {
            var command = CreateCommand();
            var room = Helpers.CreateTestRoom();
            var guest = Helpers.CreateTestUser(command.GuestId);

            _roomRepositoryMock
                .Setup(x => x.GetByIdWithAmenities(command.CreateDto.RoomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(room);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(command.GuestId.ToString()))
                .ReturnsAsync(guest);

            _bookingRepositoryMock
                .Setup(x => x.IsRoomAvailableAsync(room.Id, command.CreateDto.StartDate, command.CreateDto.EndDate, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(BookingErrors.RoomNotAvaible);
            _bookingRepositoryMock.Verify(x => x.Add(It.IsAny<Bookings>()), Times.Never);
        }

        [Fact]
        public async Task Handle_GuestIsInactive_ShouldReturnAccountInactiveFailure()
        {
            var command = CreateCommand();
            var room = Helpers.CreateTestRoom();
            var guest = Helpers.CreateTestUser(command.GuestId);
            guest.IsActive = false;

            _roomRepositoryMock
                .Setup(x => x.GetByIdWithAmenities(command.CreateDto.RoomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(room);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(command.GuestId.ToString()))
                .ReturnsAsync(guest);

            _bookingRepositoryMock
                .Setup(x => x.IsRoomAvailableAsync(room.Id, command.CreateDto.StartDate, command.CreateDto.EndDate, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(UserErrors.AccountInactive);
            _bookingRepositoryMock.Verify(x => x.Add(It.IsAny<Bookings>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidData_ReturnsSuccess()
        {
            var command = CreateCommand();
            var room = Helpers.CreateTestRoom();
            var guest = Helpers.CreateTestUser(command.GuestId);

            _roomRepositoryMock
                .Setup(x => x.GetByIdWithAmenities(command.CreateDto.RoomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(room);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(command.GuestId.ToString()))
                .ReturnsAsync(guest);

            _bookingRepositoryMock
                .Setup(x => x.IsRoomAvailableAsync(room.Id, command.CreateDto.StartDate, command.CreateDto.EndDate, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();
            _bookingRepositoryMock.Verify(x => x.Add(It.Is<Bookings>(b => b.RoomId == room.Id && b.GuestId == guest.Id)), Times.Once);
        }

        private static CreateBookingCommand CreateCommand(DateOnly? startDate = null, DateOnly? endDate = null)
        {
            var roomId = Guid.NewGuid();
            var guestId = Guid.NewGuid();

            return new CreateBookingCommand(
                new CreateBookingDto(
                    roomId,
                    startDate ?? new DateOnly(2026, 10, 10),
                    endDate ?? new DateOnly(2026, 10, 12),
                    2,
                    1),
                guestId);
        }

        private static Mock<UserManager<User>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<User>>();

            return new Mock<UserManager<User>>(
                store.Object,
                Options.Create(new IdentityOptions()),
                new PasswordHasher<User>(),
                Array.Empty<IUserValidator<User>>(),
                Array.Empty<IPasswordValidator<User>>(),
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                null!,
                new Mock<ILogger<UserManager<User>>>().Object);
        }
    }
}
