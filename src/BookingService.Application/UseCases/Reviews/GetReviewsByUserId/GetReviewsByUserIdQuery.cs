using Booking.Application.Abstractions;
using Booking.Application.DTOs.Reviews;

namespace Booking.Application.UseCases.Reviews.GetReviewsByUserId
{
    public sealed record GetReviewsByUserIdQuery(int page, int pageSize, Guid UserId) : IQuery<IReadOnlyList<ReviewResponseDto>>;
}
