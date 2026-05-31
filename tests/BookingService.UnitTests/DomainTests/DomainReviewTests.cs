using Booking.Domain.Entities;
using Booking.Domain.Enums;
using FluentAssertions;

namespace BookingService.UnitTests.DomainTests
{
    public class DomainReviewTests
    {
        [Fact]
        public void Create_ValidParams_IsSuccessTrue()
        {
            var userId = Guid.NewGuid();
            var targetId = Guid.NewGuid();
            var targetType = ReviewsTargetType.Room;
            var score = 5;
            var text = "Amazing place, highly recommend!";

            var result = Review.Create(userId, targetId, targetType, score, text);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.UserId.Should().Be(userId);
            result.Value.TargetId.Should().Be(targetId);
            result.Value.TargetType.Should().Be(targetType);
            result.Value.Score.Should().Be(score);
            result.Value.Text.Should().Be(text);
            result.Value.IsEdited.Should().BeFalse();
            result.Value.Id.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        [InlineData(-1)]
        public void Create_InvalidScore_IsSuccessFalse(int invalidScore)
        {
            var result = Review.Create(Guid.NewGuid(), Guid.NewGuid(), ReviewsTargetType.Room, invalidScore, "Valid review text length.");

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Review.InvalidScore");
            result.Value.Should().BeNull();
        }

        [Fact]
        public void Create_EmptyUserId_IsSuccessFalse()
        {
            var result = Review.Create(Guid.Empty, Guid.NewGuid(), ReviewsTargetType.Room, 5, "Valid review text length.");

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Review.EmptyUserId");
            result.Value.Should().BeNull();
        }

        [Fact]
        public void Create_EmptyTargetId_IsSuccessFalse()
        {
            var result = Review.Create(Guid.NewGuid(), Guid.Empty, ReviewsTargetType.Room, 5, "Valid review text length.");

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Review.EmptyTargetId");
            result.Value.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_EmptyText_IsSuccessFalse(string invalidText)
        {
            var result = Review.Create(Guid.NewGuid(), Guid.NewGuid(), ReviewsTargetType.Room, 5, invalidText);

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Review.EmptyText");
            result.Value.Should().BeNull();
        }

        [Fact]
        public void Create_TextTooShort_IsSuccessFalse()
        {
            var result = Review.Create(Guid.NewGuid(), Guid.NewGuid(), ReviewsTargetType.Room, 5, "Short");

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Review.TextTooShort");
            result.Value.Should().BeNull();
        }

        [Fact]
        public void Create_TextTooLong_IsSuccessFalse()
        {
            var longText = new string('a', 1001);

            var result = Review.Create(Guid.NewGuid(), Guid.NewGuid(), ReviewsTargetType.Room, 5, longText);

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Review.TextTooLong");
            result.Value.Should().BeNull();
        }

        [Fact]
        public void Create_InvalidTargetType_IsSuccessFalse()
        {
            var invalidTargetType = (ReviewsTargetType)999;

            var result = Review.Create(Guid.NewGuid(), Guid.NewGuid(), invalidTargetType, 5, "Valid review text length.");

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Review.InvalidTargetType");
            result.Value.Should().BeNull();
        }

        [Fact]
        public void UpdateScore_ValidNewScore_IsSuccessTrueAndSetsIsEdited()
        {
            var review = Helpers.CreateTestReview(score: 4);

            var result = review.UpdateScore(5);

            result.IsSuccess.Should().BeTrue();
            review.Score.Should().Be(5);
            review.IsEdited.Should().BeTrue();
        }

        [Fact]
        public void UpdateScore_SameScore_IsSuccessTrueAndIsEditedStaysFalse()
        {
            var review = Helpers.CreateTestReview(score: 4);

            var result = review.UpdateScore(4);

            result.IsSuccess.Should().BeTrue();
            review.Score.Should().Be(4);
            review.IsEdited.Should().BeFalse();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void UpdateScore_InvalidScore_IsSuccessFalse(int invalidScore)
        {
            var review = Helpers.CreateTestReview(score: 4);

            var result = review.UpdateScore(invalidScore);

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Review.InvalidScore");
            review.Score.Should().Be(4);
            review.IsEdited.Should().BeFalse();
        }

        [Fact]
        public void UpdateScore_AlreadyEdited_IsSuccessFalse()
        {
            var review = Helpers.CreateTestReview(score: 4);
            review.UpdateScore(5);

            var result = review.UpdateScore(3);

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Review.AlreadyEdited");
            review.Score.Should().Be(5);
        }

        [Fact]
        public void UpdateText_ValidNewText_IsSuccessTrueAndSetsIsEdited()
        {
            var review = Helpers.CreateTestReview(text: "Old text version.");
            var newText = "New updated text version.";

            var result = review.UpdateText(newText);

            result.IsSuccess.Should().BeTrue();
            review.Text.Should().Be(newText);
            review.IsEdited.Should().BeTrue();
        }

        [Fact]
        public void UpdateText_SameText_IsSuccessTrueAndIsEditedStaysFalse()
        {
            var text = "Identical text version.";
            var review = Helpers.CreateTestReview(text: text);

            var result = review.UpdateText(text);

            result.IsSuccess.Should().BeTrue();
            review.Text.Should().Be(text);
            review.IsEdited.Should().BeFalse();
        }

        [Fact]
        public void UpdateText_TextTooShort_IsSuccessFalse()
        {
            var review = Helpers.CreateTestReview(text: "Valid original text version.");

            var result = review.UpdateText("Short");

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Review.TextTooShort");
            review.IsEdited.Should().BeFalse();
        }

        [Fact]
        public void UpdateText_TextTooLong_IsSuccessFalse()
        {
            var review = Helpers.CreateTestReview(text: "Valid original text version.");
            var longText = new string('a', 1001);

            var result = review.UpdateText(longText);

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Review.TextTooLong");
            review.IsEdited.Should().BeFalse();
        }

        [Fact]
        public void UpdateText_AlreadyEdited_IsSuccessFalse()
        {
            var review = Helpers.CreateTestReview();
            review.UpdateText("First valid modification.");

            var result = review.UpdateText("Second modification attempt.");

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Review.AlreadyEdited");
        }
    }
}
