using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Domain.Common;

namespace Booking.Domain.Errors
{
    public class AddressErrors
    {
        public static readonly Error InvalidAddress =
            new Error("Address.InvalidAddress", "Inputed address is invalid");
    }
}
