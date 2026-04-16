using Booking.Domain.Common;

namespace Booking.Domain.Errors
{
    public class AmenityErrors
    {
        public static readonly Error NotFound =
            new Error("Amenity.NotFound", "Amenity was not found");

        public static readonly Error EmptyName =
            new Error("Amenity.EmptyTitle", "Name is empty");

        public static readonly Error CreationProblem =
            new Error("Amenity.CreationProblem", "Occured creation problem");
    }
}
