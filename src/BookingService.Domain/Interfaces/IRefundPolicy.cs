using Booking.Domain.Entities;
using Booking.Domain.ValueObjects;

namespace Booking.Domain.Interfaces
{
    public interface IRefundPolicy
    {
        public RefundValue CalculateRefund(Bookings booking, DateTime nowUtc);
    }
}
