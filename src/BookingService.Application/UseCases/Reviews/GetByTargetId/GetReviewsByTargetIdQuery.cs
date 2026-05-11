using Booking.Application.Abstractions;
using Booking.Application.DTOs.Reviews;

namespace Booking.Application.UseCases.Reviews.GetByTargetId
{
    public sealed record GetReviewsByTargetIdQuery(int page, int pageSize, Guid TargetId) : IQuery<IReadOnlyList<ReviewResponseDto>>;
}
