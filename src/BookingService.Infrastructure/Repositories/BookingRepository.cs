using Booking.Domain.Entities;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Infrastructure.Data;

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
    }
}
