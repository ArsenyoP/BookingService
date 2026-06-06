using Booking.Application.DTOs.Reviews;
using Booking.Application.UseCases.Reviews.CreateReview;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BookingService.IntegrationTests.IntegrationTests.ReviewIntegrationTests
{
    public class CreateReviewIntegrationTests : BaseIntegrationTest
    {
        private readonly Helpers helpers;

        public CreateReviewIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
            helpers = new Helpers(Sender, DbContext);
        }

        [Fact]
        public async Task CreateRoomReview_ValidData_ShouldReturnSuccess()
        {
            var userId = await helpers.CreateTestUser();
            var listingId = await helpers.CreateTestListing();
            var roomId = await helpers.CreateTestRoom(listingId);

            var booking = await helpers.CreateTestBooking(roomId, userId, listingId, true);

            var createReviewDto = new CreateReviewDto(roomId, Booking.Domain.Enums.ReviewsTargetType.Room, 5, "Some text for test room review");
            var command = new CreateReviewCommand(createReviewDto, userId.ToString());


            var result = await Sender.Send(command);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBe(Guid.Empty);

            var reviewInDb = await DbContext.Review
                .FirstOrDefaultAsync(r => r.Id == result.Value);

            reviewInDb.Should().NotBeNull();
            reviewInDb!.Score.Should().Be(5);
            reviewInDb.Text.Should().Be("Some text for test room review");
            reviewInDb.UserId.Should().Be(userId);
            reviewInDb.TargetId.Should().Be(roomId);
            reviewInDb.TargetType.Should().Be(Booking.Domain.Enums.ReviewsTargetType.Room);

            var roomInDb = await DbContext.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == roomId);

            roomInDb.Should().NotBeNull();
            roomInDb!.ReviewsCount.Should().Be(1);
            roomInDb.AverageRating.Should().Be(5.0m);
        }

        [Fact]
        public async Task CreateRoomReview_HasNooBooking_ShouldReturnSuccess()
        {
            var userId = await helpers.CreateTestUser();
            var listingId = await helpers.CreateTestListing();
            var roomId = await helpers.CreateTestRoom(listingId);


            var createReviewDto = new CreateReviewDto(roomId, Booking.Domain.Enums.ReviewsTargetType.Room, 5, "Some text for test room review");
            var command = new CreateReviewCommand(createReviewDto, userId.ToString());


            var result = await Sender.Send(command);


            result.IsSuccess.Should().BeFalse();

            var reviewInDb = await DbContext.Review
                .FirstOrDefaultAsync(r => r.Id == result.Value);

            reviewInDb.Should().BeNull();
        }
    }
}
