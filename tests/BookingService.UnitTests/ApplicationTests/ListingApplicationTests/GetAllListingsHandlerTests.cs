using Booking.Application.DTOs.Listings;
using Booking.Application.Helpers.Room;
using Booking.Application.Interfaces.IQueries;
using Booking.Application.UseCases.Listing.GetAllListings;
using FluentAssertions;
using Moq;

namespace BookingService.UnitTests.ApplicationTests.ListingApplicationTests
{
    public class GetAllListingsHandlerTests
    {
        private readonly Mock<IListingQueries> _listingQueriesMock;
        private readonly GetAllListingsHandler _handler;

        public GetAllListingsHandlerTests()
        {
            _listingQueriesMock = new Mock<IListingQueries>();
            _handler = new GetAllListingsHandler(_listingQueriesMock.Object);
        }

        [Fact]
        public async Task Handle_ValidQuery_ReturnsSuccessWithListings()
        {
            var queryObject = new ListingQueryObject(
                Title: "Apartment",
                PageSize: 10,
                Page: 1
            );
            var query = new GetAllListingsQuery(queryObject);

            IReadOnlyList<ListingResponseDto> expectedListings = new List<ListingResponseDto>();

            _listingQueriesMock
                .Setup(x => x.GetAllPagedAsync(query.QueryObject, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedListings);

            var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().Be(true);
            result.Value.Should().BeEquivalentTo(expectedListings);
            _listingQueriesMock.Verify(x => x.GetAllPagedAsync(query.QueryObject, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
