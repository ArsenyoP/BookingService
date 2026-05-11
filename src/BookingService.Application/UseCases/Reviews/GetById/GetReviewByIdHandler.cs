using Booking.Application.Abstractions;
using Booking.Application.DTOs.Reviews;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Common;

namespace Booking.Application.UseCases.Reviews.GetById
{
    public sealed class GetReviewByIdHandler(IReviewQueries _queries)
        : IQueryHandler<GetReviewByIdQuery, ReviewResponseDto>
    {
        public async Task<Result<ReviewResponseDto>> Handle(GetReviewByIdQuery request, CancellationToken ct)
        {
            var review = await _queries.GetById(request.Id, ct);

            if (review is null)
            {
                return Result<ReviewResponseDto>.Failure(new Error(
                    "Reviews.NotFound",
                    $"Review with ID {request.Id} was not found."));
            }

            return Result<ReviewResponseDto>.Success(review);
        }
    }
}
