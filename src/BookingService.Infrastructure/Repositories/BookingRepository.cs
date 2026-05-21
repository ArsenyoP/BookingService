using Booking.Domain.Entities;
using Booking.Domain.Enums;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories
{
    public class BookingRepository(AppDbContext _dbContext) : IBookingRepository
    {
        public void Add(Bookings booking)
        {
            ArgumentNullException.ThrowIfNull(booking);
            _dbContext.Bookings.Add(booking);
        }

        public void Delete(Bookings booking)
        {
            ArgumentNullException.ThrowIfNull(booking);
            _dbContext.Bookings.Remove(booking);
        }

        public async Task<Bookings?> GetBookingEntityByConfirmationToken(string confirmationToken, CancellationToken ct = default)
        {
            var booking = await _dbContext.Bookings.Where(x => x.ConfirmationToken == confirmationToken)
                .FirstOrDefaultAsync<Bookings>();

            return booking;
        }

        public async Task<Bookings?> GetById(Guid bookingId, CancellationToken ct = default)
        {
            var result = await _dbContext.Bookings.Where(x => x.Id == bookingId).FirstOrDefaultAsync<Bookings>();

            return result;
        }

        public async Task<bool> IsRoomAvailableAsync(Guid roomId, DateOnly start, DateOnly end, CancellationToken ct = default)
        {
            var result = await _dbContext.Bookings.AnyAsync(x =>
                x.RoomId == roomId &&
                x.Status != BookingStatus.Cancelled &&
                x.Status != BookingStatus.Expired &&
                x.Period.StartDate < end &&
                x.Period.EndDate > start,
                ct
                );

            return !result;
        }
    }
}
