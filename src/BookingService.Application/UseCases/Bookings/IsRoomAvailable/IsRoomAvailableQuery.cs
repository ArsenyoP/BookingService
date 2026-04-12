using Booking.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.UseCases.Bookings.IsRoomAvailable
{
    public sealed record IsRoomAvailableQuery(Guid roomId, DateOnly start, DateOnly end) : IQuery<bool>;
}
