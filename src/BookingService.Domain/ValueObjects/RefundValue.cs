using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.ValueObjects
{
    public record RefundValue(decimal TotalBookingPrice, double PercentToRefund)
    {
        public decimal AmountToRefund = TotalBookingPrice * (decimal)PercentToRefund / 100;
        public decimal CancellationFee => TotalBookingPrice - AmountToRefund;
        public string Summary => $"Refund: {AmountToRefund}, ({PercentToRefund}%) from total cost, Fee: {CancellationFee}";
    }
}
