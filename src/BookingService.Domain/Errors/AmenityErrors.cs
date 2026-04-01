using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Domain.Common;

namespace Booking.Domain.Errors
{
    public class AmenityErrors
    {
        public static readonly Error EmptyName =
            new Error("Amenity.EmptyTitle", "Name is empty");
    }
}
