using Booking.Domain.Entities;

namespace Booking.Domain.Interfaces.IRepositories
{
    public interface IRoomRepository : IBaseRepository<Room>
    {
        Task<IReadOnlyList<Room>> GetByListingIdAsync(Guid listingId, CancellationToken ct = default);
        Task<IReadOnlyList<Room>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    }
}
