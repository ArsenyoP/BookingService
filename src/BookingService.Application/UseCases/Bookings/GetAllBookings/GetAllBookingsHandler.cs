using Booking.Application.Abstractions;
using Booking.Application.DTOs.Bookings;
using Booking.Domain.Common;
using Booking.Domain.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.UseCases.Bookings.GetAllBookings
{
    internal class GetAllBookingsHandler(IBookingRepository bookingRepository) : IQueryHandler<GetAllBookingsQuery, List<BookingResponseDto>>
    {

        public async Task<Result<List<BookingResponseDto>>> Handle(GetAllBookingsQuery request, CancellationToken ct)
        {
            var allBookings = await bookingRepository.GetAllAsync(ct);

            var bookingsRespone = allBookings.Select(b => new BookingResponseDto
            (
                b.Id,
                b.RoomId,
                b.GuestId,
                b.Period.StartDate,
                b.Period.EndDate,
                b.Period.TotalNights,
                b.PricePerNight,
                b.TotalPrice,
                b.AdultsCount,
                b.ChildrenCount,
                b.Status.ToString()
            )).ToList();

            return Result<List<BookingResponseDto>>.Success(bookingsRespone);
        }
    }
}
