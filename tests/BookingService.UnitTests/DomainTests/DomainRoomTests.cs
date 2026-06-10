using Booking.Domain.Entities;
using Booking.Domain.Enums;
using FluentAssertions;

namespace BookingService.UnitTests.DomainTests
{
    public class DomainRoomTests
    {
        private static Room CreateTestRoom(decimal price = 100, int adults = 2, int children = 1)
        {
            var room = Room.Create("Deluxe Room", "A beautiful room", RoomType.Deluxe, price, adults, children, Guid.NewGuid()).Value!;

            return room;
        }

        private static Amenity CreateTestAmenity()
        {
            return Amenity.Create("TV", AmenityCategory.Entertainment).Value!;
        }

        [Fact]
        public void Create_ValidParams_IsSuccessTrue()
        {
            var title = "Standard Room";
            var description = "Comfortable standard room";
            var type = RoomType.Standard;
            var price = 150m;
            var adults = 2;
            var children = 0;
            var listingId = Guid.NewGuid();

            var result = Room.Create(title, description, type, price, adults, children, listingId);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Title.Should().Be(title);
            result.Value.Description.Should().Be(description);
            result.Value.Type.Should().Be(type);
            result.Value.PricePerNight.Should().Be(price);
            result.Value.AdultsCapacity.Should().Be(adults);
            result.Value.ChildrenCapacity.Should().Be(children);
            result.Value.ListingId.Should().Be(listingId);
            result.Value.Id.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_EmptyOrNullTitle_IsSuccessFalse(string invalidTitle)
        {
            var result = Room.Create(invalidTitle, "Description", RoomType.Standard, 100, 2, 0, Guid.NewGuid());

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Room.EmptyTitle");
            result.Value.Should().BeNull();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-50)]
        public void Create_InvalidPrice_IsSuccessFalse(decimal invalidPrice)
        {
            var result = Room.Create("Title", "Description", RoomType.Standard, invalidPrice, 2, 0, Guid.NewGuid());

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Room.NegativePrice");
            result.Value.Should().BeNull();
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, 0)]
        [InlineData(0, -1)]
        public void Create_InvalidTotalCapacity_IsSuccessFalse(int adults, int children)
        {
            var result = Room.Create("Title", "Description", RoomType.Standard, 100, adults, children, Guid.NewGuid());

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("BookingErrors.NegativeNumberCapacity");
            result.Value.Should().BeNull();
        }

        [Fact]
        public void AddAmenity_NewAmenity_IsSuccessTrue()
        {
            var room = CreateTestRoom();
            var amenity = CreateTestAmenity();
            var listing = Helpers.CreateTestListing();

            room.SetListing(listing);
            var result = room.AddAmentity(amenity);

            result.IsSuccess.Should().BeTrue();
            room.Amenities.Should().ContainSingle(x => x.Id == amenity.Id);
        }

        [Fact]
        public void AddAmenity_DuplicateAmenity_IsSuccessFalse()
        {
            var room = CreateTestRoom();
            var amenity = CreateTestAmenity();
            var listing = Helpers.CreateTestListing();
            room.SetListing(listing);

            room.AddAmentity(amenity);
            var result = room.AddAmentity(amenity);

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Room.AmenityAlreadyExists");
            room.Amenities.Should().ContainSingle(x => x.Id == amenity.Id);
        }

        [Fact]
        public void RemoveAmenity_ExistingAmenity_IsSuccessTrue()
        {
            var room = CreateTestRoom();
            var amenity = CreateTestAmenity();
            var listing = Helpers.CreateTestListing();
            room.SetListing(listing);

            room.AddAmentity(amenity);
            var result = room.RemoveAmenity(amenity);

            result.IsSuccess.Should().BeTrue();
            room.Amenities.Should().NotContain(x => x.Id == amenity.Id);
        }

        [Fact]
        public void RemoveAmenity_NonExistingAmenity_IsSuccessFalse()
        {
            var room = CreateTestRoom();
            var amenity = CreateTestAmenity();

            var result = room.RemoveAmenity(amenity);

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Room.DoesntContainAmenity");
        }

        [Fact]
        public void UpdateRating_FirstReview_CalculatesCorrectAverage()
        {
            var room = CreateTestRoom();

            var result = room.UpdateRating(5);

            result.IsSuccess.Should().BeTrue();
            room.AverageRating.Should().Be(5);
            room.ReviewsCount.Should().Be(1);
        }

        [Fact]
        public void UpdateRating_MultipleReviews_CalculatesCorrectAverage()
        {
            var room = CreateTestRoom();
            room.UpdateRating(4);

            var result = room.UpdateRating(2);

            result.IsSuccess.Should().BeTrue();
            room.AverageRating.Should().Be(3);
            room.ReviewsCount.Should().Be(2);
        }
    }
}
