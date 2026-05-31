using Booking.Domain.Entities;
using Booking.Domain.Enums;
using FluentAssertions;

namespace BookingService.UnitTests.DomainTests.BookingTests
{
    public class ListingDomainTests
    {
        [Fact]
        public void Create_ValidParams_IsSuccessTrue()
        {
            var title = "Cozy Apartment";
            var description = "Nice place to stay";
            var address = Helpers.CreateTestAddress();
            var type = ListingType.Apartment;

            var result = Listing.Create(title, description, address, type);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Title.Should().Be(title);
            result.Value.Description.Should().Be(description);
            result.Value.Address.Should().Be(address);
            result.Value.ListingType.Should().Be(type);
            result.Value.Id.Should().NotBeEmpty();
        }
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_EmptyOrNullTitle_IsSuccessFalse(string invalidTitle)
        {
            var description = "Nice place to stay";
            var address = Helpers.CreateTestAddress();

            var result = Listing.Create(invalidTitle, description, address, ListingType.Apartment);

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Listing.EmptyTitle");
            result.Value.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_EmptyOrNullDescription_IsSuccessFalse(string invalidDescription)
        {
            var title = "Cozy Apartment";
            var address = Helpers.CreateTestAddress();

            var result = Listing.Create(title, invalidDescription, address, ListingType.Apartment);

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Listing.EmptyDescription");
            result.Value.Should().BeNull();
        }

        [Fact]
        public void Create_NullAddress_IsSuccessFalse()
        {
            var result = Listing.Create("Cozy Apartment", "Nice place to stay", null!, ListingType.Apartment);

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Listing.AddressRequired");
            result.Value.Should().BeNull();
        }
    }
}
