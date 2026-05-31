using Booking.Domain.Entities;
using Booking.Domain.Enums;
using FluentAssertions;

namespace BookingService.UnitTests.DomainTests
{
    public class DomainAmenityTests
    {
        [Fact]
        public void Create_ValidParams_IsSuccessTrue()
        {
            var name = "Wi-Fi";
            var category = AmenityCategory.Entertainment;

            var result = Amenity.Create(name, category);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Name.Should().Be(name);
            result.Value.Category.Should().Be(category);
            result.Value.Id.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_EmptyOrNullName_IsSuccessFalse(string invalidName)
        {
            var category = AmenityCategory.Bathroom;

            var result = Amenity.Create(invalidName, category);

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Amenity.EmptyTitle");
            result.Value.Should().BeNull();
        }
    }
}
