using Booking.Application.DTOs.Reviews;

namespace Booking.Application.Interfaces.IQueries
{
    public interface IReviewQuery
    {
        public Task<IReadOnlyList<ReviewResponseDto>> GetAll(int page, int pageSize, CancellationToken ct);
        public Task<ReviewResponseDto> GetById(Guid id, CancellationToken ct);
        public Task<IReadOnlyList<ReviewResponseDto>> GetByUserId(int page, int pageSize, Guid userId, CancellationToken ct);
        public Task<IReadOnlyList<ReviewResponseDto>> GetByTargetId(int page, int pageSize, Guid targetId, CancellationToken ct);
    }
}
