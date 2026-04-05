using Booking.Application.Abstractions;
using Booking.Application.DTOs.Bookings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Booking.Application.UseCases.Bookings.GetAllBookings
{
    public sealed record GetAllBookingsQuery : IQuery<List<BookingResponseDto>>;
}