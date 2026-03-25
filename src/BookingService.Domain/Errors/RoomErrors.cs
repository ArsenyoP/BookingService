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
        public static readonly Error EmptyTitle =
            new("Room.EmptyTitle", "The room title cannot be empty");

        public static readonly Error EmptyDescription =
            new("Room.EmptyDescription", "The room description cannot be empty");

        public static readonly Error InvalidPrice =
            new("Room.InvalidPrice", "The price per night must be greater than zero");

        public static readonly Error InvalidCapacity =
            new("Room.InvalidCapacity", "Capacity must be a non-negative number");

        public static readonly Error AmenityAlreadyExists =
            new("Room.AmenityAlreadyExists", "This amenity has already been added to the room");

        public static readonly Error NotFound =
            new("Room.NotFound", "The room with the specified ID was not found");
    }
}
