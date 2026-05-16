using Booking.Application.Abstractions;
using Booking.Application.DTOs.Reviews;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Common;

namespace Booking.Application.UseCases.Reviews.GetByTargetId
{
    public sealed class GetReviewsByTargetIdHandler(IReviewQueries _queries) : IQueryHandler<GetReviewsByTargetIdQuery, IReadOnlyList<ReviewResponseDto>>
    {
        public async Task<Result<IReadOnlyList<ReviewResponseDto>>> Handle(GetReviewsByTargetIdQuery request, CancellationToken ct)
        {
            var page = request.page < 1 ? 1 : request.page;
            var pageSize = request.pageSize < 1 || request.pageSize > 100 ? 10 : request.pageSize;

            var reviews = await _queries.GetByTargetId(page, pageSize, request.TargetId, ct);
            return Result<IReadOnlyList<ReviewResponseDto>>.Success(reviews);
        }
    }
}
