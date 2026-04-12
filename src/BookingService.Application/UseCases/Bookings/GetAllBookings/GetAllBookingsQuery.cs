using Booking.Application.Abstractions;
using Booking.Application.DTOs.Bookings;



namespace Booking.Application.UseCases.Bookings.GetAllBookings
{
    public sealed record GetAllBookingsQuery(int Page, int PageSize) : IQuery<IReadOnlyList<BookingResponseDto>>;
}