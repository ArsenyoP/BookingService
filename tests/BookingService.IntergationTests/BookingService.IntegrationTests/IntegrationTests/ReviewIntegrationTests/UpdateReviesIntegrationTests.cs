using Booking.Application.DTOs.Reviews;
using Booking.Application.UseCases.Reviews.UpdateReview;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BookingService.IntegrationTests.IntegrationTests.ReviewIntegrationTests
{
    public class UpdateReviesIntegrationTests : BaseIntegrationTest
    {
        private readonly Helpers helpers;
        public UpdateReviesIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
            helpers = new Helpers(Sender, DbContext);
        }

        [Fact]
        public async Task UpdateReview_ValidData_ReturnsReviewResponseDto()
        {
            var userId = await helpers.CreateTestUser();
            var listingId = await helpers.CreateTestListing();
            var roomId = await helpers.CreateTestRoom(listingId);

            var reviewDto = await helpers.CreateRoomReview(roomId, userId, listingId);

            var updateDto = new UpdateReviewDto("Changed text to room review", 1);
            var updateCommand = new UpdateReviewCommand(updateDto, userId, roomId);


            var result = await Sender.Send(updateCommand);


            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Score.Should().Be(1);
            result.Value.Text.Should().Be("Changed text to room review");
            result.Value.IsEdited.Should().BeTrue();
            result.Value.TargetType.Should().Be(Booking.Domain.Enums.ReviewsTargetType.Room.ToString());


            var reviewInDb = await DbContext.Review
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.TargetId == roomId && r.UserId == userId);

            reviewInDb.Should().NotBeNull();
            reviewInDb!.Score.Should().Be(1);
            reviewInDb.Text.Should().Be("Changed text to room review");

            var roomInDb = await DbContext.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == roomId);

            roomInDb.Should().NotBeNull();
            roomInDb!.ReviewsCount.Should().Be(1);
            roomInDb.AverageRating.Should().Be(1.0m);
        }

        [Fact]
        public async Task UpdateReview_ReviewDoesNotExist_ShouldReturnFailure()
        {
            var userId = await helpers.CreateTestUser();
            var listingId = await helpers.CreateTestListing();
            var roomId = await helpers.CreateTestRoom(listingId);

            var updateDto = new UpdateReviewDto("Changed text to room review", 3);
            var updateCommand = new UpdateReviewCommand(updateDto, userId, roomId);

            var result = await Sender.Send(updateCommand);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateReview_AlreadyEdited_ShouldReturnFailure()
        {
            var userId = await helpers.CreateTestUser();
            var listingId = await helpers.CreateTestListing();
            var roomId = await helpers.CreateTestRoom(listingId);
            await helpers.CreateRoomReview(roomId, userId, listingId);

            var firstUpdateDto = new UpdateReviewDto("First valid change text", 4);
            var firstCommand = new UpdateReviewCommand(firstUpdateDto, userId, roomId);
            await Sender.Send(firstCommand);

            var secondUpdateDto = new UpdateReviewDto("Second change text attempt", 2);
            var secondCommand = new UpdateReviewCommand(secondUpdateDto, userId, roomId);

            var result = await Sender.Send(secondCommand);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateReview_ScoreIsInvalid_ShouldReturnFailure()
        {
            var userId = await helpers.CreateTestUser();
            var listingId = await helpers.CreateTestListing();
            var roomId = await helpers.CreateTestRoom(listingId);
            await helpers.CreateRoomReview(roomId, userId, listingId);

            var updateDto = new UpdateReviewDto("Changed text to room review", 6);
            var updateCommand = new UpdateReviewCommand(updateDto, userId, roomId);

            var result = await Sender.Send(updateCommand);

            result.IsSuccess.Should().BeFalse();

            var reviewInDb = await DbContext.Review
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.TargetId == roomId && r.UserId == userId);

            reviewInDb!.Score.Should().NotBe(6);
        }

        [Fact]
        public async Task UpdateReview_TextTooShort_ShouldReturnFailure()
        {
            var userId = await helpers.CreateTestUser();
            var listingId = await helpers.CreateTestListing();
            var roomId = await helpers.CreateTestRoom(listingId);
            await helpers.CreateRoomReview(roomId, userId, listingId);

            var updateDto = new UpdateReviewDto("Short", 4);
            var updateCommand = new UpdateReviewCommand(updateDto, userId, roomId);

            var result = await Sender.Send(updateCommand);

            result.IsSuccess.Should().BeFalse();

            var reviewInDb = await DbContext.Review
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.TargetId == roomId && r.UserId == userId);

            reviewInDb!.Text.Should().NotBe("Short");
        }
    }
}
