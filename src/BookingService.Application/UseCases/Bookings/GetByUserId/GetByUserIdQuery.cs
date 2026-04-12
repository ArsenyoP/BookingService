using Booking.Application.Abstractions;
using Booking.Application.DTOs.Bookings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.UseCases.Bookings.GetByUserId
{
    public sealed record GetByUserIdQuery(Guid userId, int page, int pageSize) : IQuery<IReadOnlyList<BookingResponseDto>>;
}
