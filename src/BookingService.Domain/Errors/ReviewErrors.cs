using Booking.Domain.Common;

namespace Booking.Domain.Errors
{
    public class ReviewErrors
    {
        public static readonly Error InvalidScore =
            new Error("Review.InvalidScore", "Score must be in the range beetween 1 and 5");

        public static readonly Error TextTooShort =
            new Error("Review.TextTooShort", "Text must be longer than 10 symbols");

        public static readonly Error TextTooLong =
            new Error("Review.TextTooLong", "Text must be shorter than 1000 symbols");

        public static readonly Error InvalidTargetType =
            new Error("Review.InvalidTargetType", "Invalid target type. Available: Room, Listing");

        public static readonly Error EmptyText =
            new Error("Review.EmptyText", "Text must contain a value in range beetween 10 and 1000 symbols");

        public static readonly Error EmptyUserId =
            new Error("Review.EmptyUserId", "User's id can't be empty");

        public static readonly Error EmptyTargetId =
            new Error("Review.EmptyTargetId", "Target's id can't be empty");
    }
}
