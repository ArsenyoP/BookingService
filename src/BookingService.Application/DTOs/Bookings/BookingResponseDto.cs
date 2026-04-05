using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.DTOs.Bookings
{
    public record BookingResponseDto(Guid Id,
        Guid RoomId,
        Guid GuestId,
        DateOnly StartDate,
        DateOnly EndDate,
        int TotalNights,
        decimal PricePerNight,
        decimal TotalPrice,
        int AdultsCount,
        int ChildrenCount,
        string Status)
    { }
}
