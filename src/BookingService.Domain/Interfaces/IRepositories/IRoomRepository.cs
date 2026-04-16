using Booking.Domain.Entities;

namespace Booking.Domain.Interfaces.IRepositories
{
    public interface IRoomRepository : IBaseRepository<Room>
    {
        public Task<Room?> GetByIdWithAmenities(Guid Id, CancellationToken ct = default);
    }
}
