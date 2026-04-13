using Booking.Application.Abstractions;
using Booking.Application.DTOs.Bookings;


namespace Booking.Application.UseCases.Bookings.GetById
{
    public sealed record GetByIdQuery(Guid id) : IQuery<BookingResponseDto>;
}
