using Booking.Application.Abstractions;
using Booking.Application.DTOs.Reviews;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Common;

namespace Booking.Application.UseCases.Reviews.GetAllReviews
{
    public class GetAllReviewsHandler(IReviewQueries _queries) : IQueryHandler<GetAllReviewsQuery, IReadOnlyList<ReviewResponseDto>>
    {
        public async Task<Result<IReadOnlyList<ReviewResponseDto>>> Handle(GetAllReviewsQuery request, CancellationToken ct)
        {
            var page = request.page < 1 ? 1 : request.page;
            var pageSize = request.pageSize < 1 ? 10 : request.pageSize;

            var reviews = await _queries.GetAll(page, pageSize, ct);

            return Result<IReadOnlyList<ReviewResponseDto>>.Success(reviews);
        }
    }
}
