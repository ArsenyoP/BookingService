using Booking.Application.Interfaces;
using Booking.Application.Interfaces.IQueries;
using Booking.Application.UseCases.Amenities.AddAmenityToRoom;
using Booking.Domain.Entities;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;
using BookingService.UnitTests.DomainTests;
using FluentAssertions;
using Moq;

namespace BookingService.UnitTests.ApplicationTests.AmenitiesApplicationTests
{
    public class AddAmenityToRoomHandlerTests
    {
        private readonly Mock<IAmenityQueries> _amenityQueriesMock;
        private readonly Mock<IRoomRepository> _roomRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly AddAmenityToRoomHandler _handler;

        public AddAmenityToRoomHandlerTests()
        {
            _amenityQueriesMock = new Mock<IAmenityQueries>();
            _roomRepositoryMock = new Mock<IRoomRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new AddAmenityToRoomHandler(
                _amenityQueriesMock.Object,
                _roomRepositoryMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_ValidData_ReturnsSuccess()
        {
            var room = Helpers.CreateTestRoom();
            var amenity = Helpers.CreateTestAmenity("Wi-Fi");
            var command = new AddAmenityToRoomCommand(room.Id, amenity.Name);
            var listing = Helpers.CreateTestListing();
            room.SetListing(listing);

            _roomRepositoryMock.Setup(x => x.GetByIdWithAmenities(command.RoomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(room);
            _amenityQueriesMock.Setup(x => x.GetByNameAsync(command.AmenityName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(amenity);

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().Be(true);
            result.Value.Should().Be(amenity.Id);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_AmenityNotFound_ShouldReturnAmenityNotFoundFailure()
        {
            var command = new AddAmenityToRoomCommand(Guid.NewGuid(), "Wi-Fi");

            _amenityQueriesMock
                .Setup(x => x.GetByNameAsync(command.AmenityName, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Amenity)null!);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(AmenityErrors.NotFound);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_RoomNotFound_ShouldReturnRoomNotFoundFailure()
        {
            var command = new AddAmenityToRoomCommand(Guid.NewGuid(), "Wi-Fi");
            var amenity = Helpers.CreateTestAmenity("Wi-Fi");

            _amenityQueriesMock
                .Setup(x => x.GetByNameAsync(command.AmenityName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(amenity);

            _roomRepositoryMock
                .Setup(x => x.GetByIdWithAmenities(command.RoomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Room)null!);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(RoomErrors.NotFound);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_AmenityAlreadyAddedInRoom_ShouldReturnDomainFailure()
        {
            var command = new AddAmenityToRoomCommand(Guid.NewGuid(), "Wi-Fi");
            var amenity = Helpers.CreateTestAmenity("Wi-Fi");
            var room = Helpers.CreateTestRoom();
            var listing = Helpers.CreateTestListing();

            room.SetListing(listing);
            room.AddAmentity(amenity);

            _amenityQueriesMock
                .Setup(x => x.GetByNameAsync(command.AmenityName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(amenity);

            _roomRepositoryMock
                .Setup(x => x.GetByIdWithAmenities(command.RoomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(room);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(RoomErrors.AmenityAlreadyExists);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
