using Booking.Domain.Entities;

namespace Booking.Domain.Interfaces.IRepositories
{
    public interface IBookingRepository : IBaseRepository<Bookings>
    {
        public Task<bool> IsRoomAvailableAsync(Guid roomId, DateOnly start, DateOnly end, CancellationToken ct = default);
    }
}
