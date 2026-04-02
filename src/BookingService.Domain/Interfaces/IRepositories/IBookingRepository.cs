using Booking.Domain.Entities;
using Booking.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Interfaces.IRepositories
{
    public interface IBookingRepository : IBaseRepository<Bookings>
    {
        public Task<IReadOnlyList<Bookings?>> GetBookingsByUserId(Guid userId);
        public Task<IReadOnlyList<Bookings?>> GetBookingsByRoomId(Guid rooomId);
        public bool IsRoomAvaible(DateRange period, Guid roomId);
    }
}
