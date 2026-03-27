using Booking.Domain.Common;
using Booking.Domain.Entities;
using Booking.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Interfaces
{
    public interface IRefundPolicy
    {
        public RefundValue CalculateRefund(Entities.Booking booking, DateTime nowUtc);
    }
}
