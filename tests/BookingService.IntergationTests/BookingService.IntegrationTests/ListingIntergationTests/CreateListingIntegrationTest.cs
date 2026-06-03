using Booking.Application.UseCases.Listing.CreateListing;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BookingService.IntegrationTests.ListingIntergationTests
{
    public class CreateListingIntegrationTest : BaseIntegrationTest
    {
        public CreateListingIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task Create_ShouldAddNewListingToDatabase_WhenCommandIsValid()
        {
            var command = new CreateListingCommand
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


            var resultId = await Sender.Send(command);

            var listingInDb = await DbContext.Listings.FindAsync(resultId.Value);
            var listings = await DbContext.Listings.ToListAsync();

            listingInDb.Should().NotBeNull();
            listingInDb!.Title.Should().Be(command.Title);
            listingInDb.Description.Should().Be(command.Description);
            listings.Count.Should().Be(1);
        }

        [Fact]
        public async Task Create_DuplicateAddress_ThrowInternalError()
        {
            var command1 = new CreateListingCommand
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

            var command2 = new CreateListingCommand
                        (
                            "Затишна квартира в центрі Львову",
                            "Прекрасне житло з усім необхідним",
                            "Ukraine",
                            "Ternopil",
                            "Saharova",
                            "1",
                            5,
                            Booking.Domain.Enums.ListingType.Apartment
                        );

            var firstResult = await Sender.Send(command1);
            firstResult.IsSuccess.Should().Be(true);

            Func<Task> act = async () => await Sender.Send(command2);

            await act.Should().ThrowAsync<DbUpdateException>();

        }
    }
}
