using Booking.Domain.Common;
using Booking.Domain.ValueObjects;

namespace Booking.Domain.Entities
{
    public class Listing : Entity
    {
        public Address Address { get; private set; }


    }
}
