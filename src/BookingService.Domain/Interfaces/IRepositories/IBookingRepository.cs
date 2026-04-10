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
    }
}
