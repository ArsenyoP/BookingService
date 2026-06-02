using Booking.Application.Interfaces;
using Booking.Application.UseCases.Listing.CreateListing;
using Booking.Domain.Entities;
using Booking.Domain.Enums;
using Booking.Domain.Interfaces.IRepositories;
using FluentAssertions;
using Moq;

namespace BookingService.UnitTests.ApplicationTests.ListingApplicationTests
{
    public class CreateListingHandlerTests
    {
        private readonly Mock<IListingRepository> _listingRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateListingHandler _handler;

        public CreateListingHandlerTests()
        {
            _listingRepositoryMock = new Mock<IListingRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new CreateListingHandler(
                _listingRepositoryMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_ValidData_ReturnsSuccess()
        {
            var command = new CreateListingCommand(
                "Cozy Apartment",
                "A great place to stay near the center",
                "Ukraine",
                "Ternopil",
                "Ruska St",
                "15",
                3,
                default(ListingType)
            );

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().Be(true);
            result.Value.Should().NotBeEmpty();
            _listingRepositoryMock.Verify(x => x.Add(It.IsAny<Listing>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidAddress_ShouldReturnAddressFailure()
        {
            var command = new CreateListingCommand(
                "Cozy Apartment",
                "A great place to stay near the center",
                string.Empty,
                string.Empty,
                "Ruska St",
                "15",
                3,
                default(ListingType)
            );

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().Be(false);
            _listingRepositoryMock.Verify(x => x.Add(It.IsAny<Listing>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_InvalidListingData_ShouldReturnListingFailure()
        {
            var command = new CreateListingCommand(
                string.Empty,
                "A great place to stay near the center",
                "Ukraine",
                "Ternopil",
                "Ruska St",
                "15",
                3,
                default(ListingType)
            );

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().Be(false);
            _listingRepositoryMock.Verify(x => x.Add(It.IsAny<Listing>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
