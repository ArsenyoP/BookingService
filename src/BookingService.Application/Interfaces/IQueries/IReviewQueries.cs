using Booking.Application.DTOs.Reviews;
using Booking.Domain.Enums;
using System.Data;

namespace Booking.Application.Interfaces.IQueries
{
    public interface IReviewQueries
    {
        public Task<IReadOnlyList<ReviewResponseDto>> GetAll(int page, int pageSize, CancellationToken ct);
        public Task<ReviewResponseDto> GetById(Guid id, CancellationToken ct);
        public Task<IReadOnlyList<ReviewResponseDto>> GetByUserId(int page, int pageSize, Guid userId, CancellationToken ct);
        public Task<IReadOnlyList<ReviewResponseDto>> GetByTargetId(int page, int pageSize, Guid targetId, CancellationToken ct);
        public Task<bool> HasReviewByUserAsync(Guid userId, Guid targetId, CancellationToken ct);
        public Task<bool> HasRoomBooking(Guid userId, Guid roomId, CancellationToken ct);
        public Task<bool> HasListingBooking(Guid userId, Guid listingId, CancellationToken ct);
        public Task AddedReviewToTarget(Guid targetId, int score, ReviewsTargetType targetType, IDbTransaction transaction, CancellationToken ct);
    }
}
