using Booking.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Errors
{
    public class RoomErrors
    {
        public static readonly Error AmenityAlreadyExists =
            new("Room.AmenityAlreadyExists", "This amenity already belongs to current room");
    }
}
