using Booking.Domain.Entities;

namespace Booking.Domain.Interfaces.IRepositories
{
    public interface IListingRepository : IBaseRepository<Listing>
    {
        public Task<Listing?> GetByIdWithAmenities(Guid id, CancellationToken ct = default);
        public Task<List<Listing?>> GetByIds(IEnumerable<Guid> ids, CancellationToken ct = default);
    }
}
