using Booking.Application.Abstractions;
using Booking.Application.DTOs.Reviews;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Common;

namespace Booking.Application.UseCases.Reviews.GetReviewsByUserId
{
    public sealed class GetReviewsByUserIdHandler(IReviewQueries _queries) : IQueryHandler<GetReviewsByUserIdQuery, IReadOnlyList<ReviewResponseDto>>
    {
        public async Task<Result<IReadOnlyList<ReviewResponseDto>>> Handle(GetReviewsByUserIdQuery request, CancellationToken ct)
        {
            var page = request.page < 1 ? 1 : request.page;
            var pageSize = request.pageSize < 1 || request.pageSize > 100 ? 10 : request.pageSize;

            var reviews = await _queries.GetByUserId(page, pageSize, request.UserId, ct);
            return Result<IReadOnlyList<ReviewResponseDto>>.Success(reviews);
        }
    }
}