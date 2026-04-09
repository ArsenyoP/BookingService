using Booking.Domain.Entities;

namespace Booking.Domain.Interfaces.IRepositories
{
    public interface IListingRepository : IBaseRepository<Listing>
    {
        //Override GetById to include amenities
        Task<IReadOnlyList<Listing>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    }
}
