using Booking.Application.Abstractions;
using Booking.Application.DTOs.Reviews;

namespace Booking.Application.UseCases.Reviews.CreateReview
{
    public sealed record CreateReviewCommand(CreateReviewDto createReviewDto, string userId) : ICommand<Guid>;
}
