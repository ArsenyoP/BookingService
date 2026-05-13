using Booking.Application.DTOs.Reviews;
using Booking.Domain.Entities;

namespace Booking.Application.Mappers
{
    public static class ReviewMappers
    {
        public static ReviewResponseDto MapFromReviewToResponseDto(this Review review, string targetTitle, string userName)
        {
            return new ReviewResponseDto
            {
                CreatedAt = review.CreatedAt,
                IsEdited = review.IsEdited,
                Score = review.Score,
                TargetTitle = targetTitle,
                TargetType = review.TargetType.ToString(),
                Text = review.Text,
                UserName = userName
            };
        }
    }
}
