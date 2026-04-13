using Booking.Domain.Common;

namespace Booking.Domain.Errors
{
    public class BookingErrors
    {
        public static readonly Error NotFound =
            new("Booking.NotFound", "The boooking with the specified ID was not found");

        public static readonly Error ExceedsCapacity =
            new Error("BookingErrors.ExceedsCapacity", "Count of guests can't exceed apartment's capacity");

        public static readonly Error AtLeastOneAdultRequired =
            new("Booking.AtLeastOneAdultRequired", "At least one adult is required for a booking");

        public static readonly Error NegativeChildrenCount =
            new("Booking.NegativeChildrenCount", "The number of children cannot be negative");

        public static readonly Error CannotCancelStartedBooking =
            new Error("Booking.CannotCancelStartedBooking", "Can't cancel booking which allready started");

        public static readonly Error CannotCancel =
            new Error("Booking.CannotCancel", "Can't Cancel this booking");

        public static readonly Error NegativePricePerNight =
            new Error("Booking.NegativePricePerNight", "Price per night can't be negative");
    }
}
