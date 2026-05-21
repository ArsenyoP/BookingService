using Booking.Domain.Entities;

namespace Booking.Domain.Interfaces.IRepositories
{
    public interface IBookingRepository : IBaseRepository<Bookings>
    {
        public Task<Bookings?> GetById(Guid bookingId, CancellationToken ct = default);
        public Task<bool> IsRoomAvailableAsync(Guid roomId, DateOnly start, DateOnly end, CancellationToken ct = default);
        public Task<Bookings?> GetBookingEntityByConfirmationToken(string confirmationToken, CancellationToken ct = default);
    }
}
