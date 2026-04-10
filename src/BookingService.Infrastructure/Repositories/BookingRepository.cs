using Booking.Domain.Entities;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Domain.ValueObjects;
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
            throw new NotImplementedException();
        }
    }
}
