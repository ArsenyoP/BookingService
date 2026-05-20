using Booking.Domain.Common;
using Booking.Domain.Enums;
using Booking.Domain.Errors;

namespace Booking.Domain.Entities
{
    public class Review : AggregateRoot
    {
        public string Text { get; private set; }
        public int Score { get; private set; }
        public ReviewsTargetType TargetType { get; private set; }
        public Guid TargetId { get; private set; }
        public Guid UserId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsEdited { get; private set; } = false;

        private Review() { }

        public Review(Guid userId,
            Guid targetId,
            ReviewsTargetType targetType,
            int score, string text)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            TargetId = targetId;
            TargetType = targetType;
            Score = score;
            Text = text;
            CreatedAt = DateTime.UtcNow;
            IsEdited = false;
        }

        public static Result<Review> Create(Guid userId,
            Guid targetId,
            ReviewsTargetType targetType,
            int score, string text)
        {
            if (score <= 0 || score > 5)
            {
                return Result<Review>.Failure(ReviewErrors.InvalidScore);
            }

            if (userId == Guid.Empty)
            {
                return Result<Review>.Failure(ReviewErrors.EmptyUserId);
            }

            if (targetId == Guid.Empty)
            {
                return Result<Review>.Failure(ReviewErrors.EmptyTargetId);
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                return Result<Review>.Failure(ReviewErrors.EmptyText);
            }

            if (text.Length < 10)
            {
                return Result<Review>.Failure(ReviewErrors.TextTooShort);
            }

            if (text.Length > 1000)
            {
                return Result<Review>.Failure(ReviewErrors.TextTooLong);
            }

            if (!Enum.IsDefined(typeof(ReviewsTargetType), targetType))
            {
                return Result<Review>.Failure(ReviewErrors.InvalidTargetType);
            }

            Review review = new Review(userId, targetId, targetType, score, text);
            return Result<Review>.Success(review);
        }

        public Result<Review> UpdateScore(int newScore)
        {
            if (IsEdited) return Result<Review>.Failure(ReviewErrors.AlreadyEdited);

            if (Score == newScore)
            {
                return Result<Review>.Success(this);
            }

            if (newScore < 1 || newScore > 5)
            {
                return Result<Review>.Failure(ReviewErrors.InvalidScore);
            }

            Score = newScore;
            IsEdited = true;

            return Result<Review>.Success(this);
        }

        public Result<Review> UpdateText(string newText)
        {
            if (IsEdited) return Result<Review>.Failure(ReviewErrors.AlreadyEdited);

            if (Text == newText)
            {
                return Result<Review>.Success(this);
            }

            if (newText.Length < 10)
            {
                return Result<Review>.Failure(ReviewErrors.TextTooShort);
            }

            if (newText.Length > 1000)
            {
                return Result<Review>.Failure(ReviewErrors.TextTooLong);
            }

            Text = newText;
            IsEdited = true;

            return Result<Review>.Success(this);
        }
    }
}
