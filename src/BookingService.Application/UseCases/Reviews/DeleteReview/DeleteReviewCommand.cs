using Booking.Application.Abstractions;

namespace Booking.Application.UseCases.Reviews.DeleteReview
{
    public sealed record DeleteReviewCommand(Guid UserId, Guid TargetId) : ICommand<Guid>;
}
