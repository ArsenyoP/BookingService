using Booking.Application.Abstractions;

namespace Booking.Application.UseCases.Booking.DeleteBooking
{
    public sealed record DeleteBookingCommand(Guid BookingId) : ICommand<Guid>;
}