using Booking.Domain.Entities;
using Booking.Domain.Interfaces;
using Booking.Domain.ValueObjects;

namespace Booking.Domain.Services
{
    public class RefundPolicy : IRefundPolicy
    {
        public RefundValue CalculateRefund(Bookings booking, DateTime nowUtc)
        {
            DateOnly today = DateOnly.FromDateTime(nowUtc);
            int daysBeforeStart = booking.Period.StartDate.DayNumber - today.DayNumber;

            decimal percentToRefund = daysBeforeStart switch
            {
                >= 7 => 100,
                >= 3 => 75,
                >= 1 => 50,
                _ => 0
            };

            return new RefundValue(booking.TotalPrice, percentToRefund);
        }
    }
}
