using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.ValueObjects
{
    public record RefundValue(decimal TotalBookingPrice, decimal PercentToRefund)
    {
        public decimal AmountToRefund => TotalBookingPrice * PercentToRefund / 100;
        public decimal CancellationFee => TotalBookingPrice - AmountToRefund;
    }
}
