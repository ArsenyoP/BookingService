using Booking.Domain.Common;

namespace Booking.Domain.Errors
{
    public class AddressErrors
    {
        public static readonly Error InvalidAddress =
            new Error("Address.InvalidAddress", "Inputed address is invalid");
    }
}
