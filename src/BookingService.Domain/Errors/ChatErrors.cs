using Booking.Domain.Common;

namespace Booking.Domain.Errors
{
    public class ChatErrors
    {
        public readonly static Error EmptyMessage =
            new Error("EmptyMessage.PastStart", "Message for AI search can't be empty");
    }
}
