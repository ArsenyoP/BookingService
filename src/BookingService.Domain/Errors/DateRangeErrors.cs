using Booking.Domain.Common;


namespace Booking.Domain.Errors
{
    public class DateRangeErrors
    {
        public readonly static Error PastStart =
            new Error("Booking.PastStart", "Start date cannot be in the past");

        public readonly static Error InvalidEnd =
            new Error("Booking.InvalidEnd", "End date must be after start date");

        public readonly static Error InvalidDate =
            new Error("Booking.InvalidDate", "Date is invalide");
    }
}
