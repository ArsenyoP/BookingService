using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Enums
{
    public enum BookingStatus
    {
        Confirmed = 1,
        Pending = 2,
        Cancelled = 3,
        Completed = 4,
        Expired = 5,
    }
}
