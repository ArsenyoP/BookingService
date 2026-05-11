using Booking.Application.Abstractions;
using Booking.Application.DTOs.Reviews;

namespace Booking.Application.UseCases.Reviews.GetAllReviews
{
    public record GetAllReviewsQuery(int page, int pageSize) : IQuery<IReadOnlyList<ReviewResponseDto>>;
}
