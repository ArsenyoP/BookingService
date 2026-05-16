using Booking.Domain.Enums;

namespace Booking.Application.DTOs.Reviews
{
    public sealed record CreateReviewDto(
        Guid TargetId,
        ReviewsTargetType TargetType,
        int Score,
        string Text);
}
