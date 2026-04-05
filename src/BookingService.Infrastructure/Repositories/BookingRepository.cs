using Booking.Domain.Entities;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Domain.ValueObjects;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Repositories
{
    public class BookingRepository(AppDbContext _dbContext) : IBookingRepository
    {


        public void Add(Bookings obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(Bookings obj)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<Bookings>> GetAllAsync(CancellationToken ct = default)
        {
            var result = await _dbContext.Bookings
                .AsNoTracking()
                .ToListAsync(ct);
            return result;
        }

        public Task<IReadOnlyList<Bookings?>> GetBookingsByRoomId(Guid rooomId)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Bookings?>> GetBookingsByUserId(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<Bookings?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public bool IsRoomAvaible(DateRange period, Guid roomId)
        {
            throw new NotImplementedException();
        }
    }
}
