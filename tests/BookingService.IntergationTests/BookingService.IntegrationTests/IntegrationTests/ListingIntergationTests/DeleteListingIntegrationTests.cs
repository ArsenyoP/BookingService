using Booking.Application.UseCases.Listing.CreateListing;
using Booking.Application.UseCases.Listing.DeleteListing;
using Booking.Domain.Errors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BookingService.IntegrationTests.IntegrationTests.ListingIntergationTests
{
    public class DeleteListingIntegrationTests : BaseIntegrationTest
    {
        public DeleteListingIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task Delete_ValidData_ShoultReturnSeccess()
        {
            var commandToCreate = new CreateListingCommand
            (
                "Затишна квартира в центрі Тернополя",
                "Прекрасне житло з усім необхідним",
                "Ukraine",
                "Ternopil",
                "Saharova",
                "1",
                5,
                Booking.Domain.Enums.ListingType.Apartment
            );

            var createResult = await Sender.Send(commandToCreate);
            createResult.IsSuccess.Should().BeTrue();

            var listingId = createResult.Value;

            var commandToDelete = new DeleteListingCommand(listingId);

            var deleteResult = await Sender.Send(commandToDelete);

            deleteResult.IsSuccess.Should().BeTrue();
            deleteResult.Value.Should().Be(listingId);

            var listingInDb = await DbContext.Listings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == listingId);

            listingInDb.Should().BeNull();
        }

        [Fact]
        public async Task Delete_ShouldReturnFailure_WhenListingDoesNotExist()
        {
            var nonExistentListingId = Guid.NewGuid();
            var commandToDelete = new DeleteListingCommand(nonExistentListingId);

            var result = await Sender.Send(commandToDelete);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(ListingErrors.NotFound);
        }
    }
}
