using Booking.Application.Abstractions;
using Booking.Application.DTOs.Reviews;

namespace Booking.Application.UseCases.Reviews.GetById
{
    public sealed record GetReviewByIdQuery(Guid Id) : IQuery<ReviewResponseDto>;
}
