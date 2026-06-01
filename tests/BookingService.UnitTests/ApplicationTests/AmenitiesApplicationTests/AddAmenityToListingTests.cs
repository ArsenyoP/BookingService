using Booking.Application.Interfaces;
using Booking.Application.Interfaces.IQueries;
using Booking.Application.UseCases.Amenities.AddAmenityToListing;
using Booking.Domain.Entities;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;
using BookingService.UnitTests.DomainTests;
using FluentAssertions;
using Moq;

namespace BookingService.UnitTests.ApplicationTests.AmenitiesApplicationTests
{
    public class AddAmenityToListingTests
    {
        private readonly Mock<IListingRepository> _listingRepoMock;
        private readonly Mock<IAmenityQueries> _amenityQueriesMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly AddAmenityToListingHandler _handler;

        public AddAmenityToListingTests()
        {
            _listingRepoMock = new Mock<IListingRepository>();
            _amenityQueriesMock = new Mock<IAmenityQueries>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new AddAmenityToListingHandler(_listingRepoMock.Object,
                _amenityQueriesMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ValidData_ReturnsSuccess()
        {
            var listing = Helpers.CreateTestListing();
            var amenity = Helpers.CreateTestAmenity("Wi-Fi");
            var command = new AddAmenityToListingCommand(listing.Id, amenity.Name);

            _listingRepoMock.Setup(x => x.GetByIdWithAmenities(command.ListingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(listing);
            _amenityQueriesMock.Setup(x => x.GetByNameAsync(command.AmenityName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(amenity);

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().Be(true);
            result.Value.Should().Be(amenity.Id);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ListingNotFound_ShouldReturnListingNotFoundFailure()
        {
            var command = new AddAmenityToListingCommand(Guid.NewGuid(), "Wi-Fi");

            _listingRepoMock
                .Setup(x => x.GetByIdWithAmenities(command.ListingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Listing)null!);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(ListingErrors.NotFound);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_AmenityNotFound_ShouldReturnAmenityNotFoundFailure()
        {
            var command = new AddAmenityToListingCommand(Guid.NewGuid(), "NonExistentAmenity");
            var listing = Helpers.CreateTestListing();

            _listingRepoMock
                .Setup(x => x.GetByIdWithAmenities(command.ListingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(listing);

            _amenityQueriesMock
                .Setup(x => x.GetByNameAsync(command.AmenityName, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Amenity)null!);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(AmenityErrors.NotFound);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_AmenityAlreadyAddedInDomain_ShouldReturnDomainFailure()
        {
            var command = new AddAmenityToListingCommand(Guid.NewGuid(), "Wi-Fi");
            var listing = Helpers.CreateTestListing();
            var amenity = Helpers.CreateTestAmenity("Wi-Fi");

            listing.AddAmenity(amenity);

            _listingRepoMock
                .Setup(x => x.GetByIdWithAmenities(command.ListingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(listing);

            _amenityQueriesMock
                .Setup(x => x.GetByNameAsync(command.AmenityName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(amenity);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(ListingErrors.AmenityAlreadyAdded);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
