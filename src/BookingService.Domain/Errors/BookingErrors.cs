using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Domain.Common;

namespace Booking.Domain.Errors
{
    public class BookingErrors
    {
        public static readonly Error ExceedsCapacity =
            new Error("BookingErrors.ExceedsCapacity", "Count of guests can't exceed apartment's capacity");

        public static readonly Error CannotCancelStartedBooking =
            new Error("BookingErrors.CannotCancelStartedBooking", "Can't cancel booking which allready started");

        public static readonly Error CannotCancel =
            new Error("BookingErrors.CannotCancel", "Can't Cancel this booking");
    }
}
