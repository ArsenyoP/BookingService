using Booking.Application.Abstractions;
using Booking.Application.DTOs.Bookings;

namespace Booking.Application.UseCases.Bookings.CreateBooking
{
    public sealed record CreateBookingCommand(
        CreateBookingDto CreateDto,
        Guid GuestId) : ICommand<Guid>;
}