using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Domain.Common;

namespace Booking.Domain.Errors
{
    public class ListingErrors
    {
        public static readonly Error EmptyTitle =
            new Error("Listing.EmptyTitle", "Title is empty");

        public static readonly Error EmptyDescription =
            new Error("Listing.EmptyDescription", "Description is empty");

        public static readonly Error InvalidPricePerNight =
            new Error("Listing.InvalidPricePerNight", "Price per night can't be negative");

        public static readonly Error AddressRequired =
            new Error("Listing.AddressRequired", "Address must contain a value");

        public static readonly Error AmenityAlreadyAdded =
            new("Listing.AmenityAlreadyAdded", "This amenity has already been added to the listing");
    }
}
