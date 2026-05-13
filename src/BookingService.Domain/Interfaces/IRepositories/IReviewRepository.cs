using Booking.Domain.Entities;

namespace Booking.Domain.Interfaces.IRepositories
{
    public interface IReviewRepository : IBaseRepository<Review>
    {
        Task<Review?> GetReviewByUserIdAndTargetId(Guid userId, Guid targetId, CancellationToken ct);
    }
}
