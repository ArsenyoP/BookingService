using Booking.Application.Interfaces;
using Booking.Application.Interfaces.IQueries;
using Booking.Application.UseCases.Amenities.RemoveAmenityFromListing;
using Booking.Domain.Entities;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;
using BookingService.UnitTests.DomainTests;
using FluentAssertions;
using Moq;

namespace BookingService.UnitTests.ApplicationTests.AmenitiesApplicationTests
{
    public class RemoveAmenityFromListingHandlerTests
    {
        private readonly Mock<IListingRepository> _listingRepoMock;
        private readonly Mock<IAmenityQueries> _amenityQueriesMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly RemoveAmenityFromListingHandler _handler;

        public RemoveAmenityFromListingHandlerTests()
        {
            _listingRepoMock = new Mock<IListingRepository>();
            _amenityQueriesMock = new Mock<IAmenityQueries>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new RemoveAmenityFromListingHandler(
                _listingRepoMock.Object,
                _amenityQueriesMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_ValidData_ReturnsSuccess()
        {
            var listing = Helpers.CreateTestListing();
            var amenity = Helpers.CreateTestAmenity("Wi-Fi");
            var command = new RemoveAmenityFromListingCommand(listing.Id, amenity.Name);

            listing.AddAmenity(amenity);

            _listingRepoMock
                .Setup(x => x.GetByIdWithAmenities(command.listingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(listing);

            _amenityQueriesMock
                .Setup(x => x.GetByNameAsync(command.amenityName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(amenity);

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().Be(true);
            result.Value.Should().Be(amenity.Id);
            listing.Amenities.Should().NotContain(amenity);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ListingNotFound_ShouldReturnListingNotFoundFailure()
        {
            var command = new RemoveAmenityFromListingCommand(Guid.NewGuid(), "Wi-Fi");

            _listingRepoMock
                .Setup(x => x.GetByIdWithAmenities(command.listingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Listing)null!);

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().Be(false);
            result.Error.Should().Be(ListingErrors.NotFound);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_AmenityNotFound_ShouldReturnAmenityNotFoundFailure()
        {
            var listing = Helpers.CreateTestListing();
            var command = new RemoveAmenityFromListingCommand(listing.Id, "NonExistent");

            _listingRepoMock
                .Setup(x => x.GetByIdWithAmenities(command.listingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(listing);

            _amenityQueriesMock
                .Setup(x => x.GetByNameAsync(command.amenityName, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Amenity)null!);

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().Be(false);
            result.Error.Should().Be(AmenityErrors.NotFound);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ListingDoesNotContainAmenity_ShouldReturnDomainFailure()
        {
            var listing = Helpers.CreateTestListing();
            var amenity = Helpers.CreateTestAmenity("Wi-Fi");
            var command = new RemoveAmenityFromListingCommand(listing.Id, amenity.Name);

            _listingRepoMock
                .Setup(x => x.GetByIdWithAmenities(command.listingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(listing);

            _amenityQueriesMock
                .Setup(x => x.GetByNameAsync(command.amenityName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(amenity);

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().Be(false);
            result.Error.Should().Be(ListingErrors.DoesntContainAmenity);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
