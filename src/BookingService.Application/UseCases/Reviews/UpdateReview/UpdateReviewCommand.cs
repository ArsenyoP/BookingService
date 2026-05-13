using Booking.Application.Abstractions;
using Booking.Application.DTOs.Reviews;

namespace Booking.Application.UseCases.Reviews.UpdateReview
{
    public sealed record UpdateReviewCommand(UpdateReviewDto UpdateDto,
        Guid UserId, Guid TargetId) : ICommand<ReviewResponseDto>;
}
