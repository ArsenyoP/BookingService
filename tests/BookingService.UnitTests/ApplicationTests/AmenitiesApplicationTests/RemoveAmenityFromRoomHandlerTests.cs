using Booking.Application.Interfaces;
using Booking.Application.Interfaces.IQueries;
using Booking.Application.UseCases.Amenities.RemoveAmenityFromRoom;
using Booking.Domain.Entities;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;
using BookingService.UnitTests.DomainTests;
using FluentAssertions;
using Moq;

namespace BookingService.UnitTests.ApplicationTests.AmenitiesApplicationTests
{
    public class RemoveAmenityFromRoomHandlerTests
    {
        private readonly Mock<IRoomRepository> _roomRepositoryMock;
        private readonly Mock<IAmenityQueries> _amenityQueriesMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly RemoveAmenityFromRoomHandler _handler;

        public RemoveAmenityFromRoomHandlerTests()
        {
            _roomRepositoryMock = new Mock<IRoomRepository>();
            _amenityQueriesMock = new Mock<IAmenityQueries>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new RemoveAmenityFromRoomHandler(
                _roomRepositoryMock.Object,
                _amenityQueriesMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_ValidData_ReturnsSuccess()
        {
            var room = Helpers.CreateTestRoom();
            var amenity = Helpers.CreateTestAmenity("Wi-Fi");
            var command = ew RemoveAmenityFromRoomCommand(room.Id, amenity.Name);
            var listing = Helpers.CreateTestListing();

            room.SetListing(listing);
            room.AddAmentity(amenity);

            _roomRepositoryMock
                .Setup(x => x.GetByIdWithAmenities(command.roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(room);

            _amenityQueriesMock
                .Setup(x => x.GetByNameAsync(command.amenityName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(amenity);

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().Be(true);
            result.Value.Should().Be(amenity.Id);
            room.Amenities.Should().NotContain(amenity);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_RoomNotFound_ShouldReturnRoomNotFoundFailure()
        {
            var command = new RemoveAmenityFromRoomCommand(Guid.NewGuid(), "Wi-Fi");

            _roomRepositoryMock
                .Setup(x => x.GetByIdWithAmenities(command.roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Room)null!);

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().Be(false);
            result.Error.Should().Be(RoomErrors.NotFound);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_AmenityNotFound_ShouldReturnAmenityNotFoundFailure()
        {
            var room = Helpers.CreateTestRoom();
            var command = new RemoveAmenityFromRoomCommand(room.Id, "NonExistent");

            _roomRepositoryMock
                .Setup(x => x.GetByIdWithAmenities(command.roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(room);

            _amenityQueriesMock
                .Setup(x => x.GetByNameAsync(command.amenityName, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Amenity)null!);

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().Be(false);
            result.Error.Should().Be(AmenityErrors.NotFound);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_RoomDoesNotContainAmenity_ShouldReturnDomainFailure()
        {
            var room = Helpers.CreateTestRoom();
            var amenity = Helpers.CreateTestAmenity("Wi-Fi");
            var command = new RemoveAmenityFromRoomCommand(room.Id, amenity.Name);

            _roomRepositoryMock
                .Setup(x => x.GetByIdWithAmenities(command.roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(room);

            _amenityQueriesMock
                .Setup(x => x.GetByNameAsync(command.amenityName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(amenity);

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().Be(false);
            result.Error.Should().Be(RoomErrors.DoesntContainAmenity);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
