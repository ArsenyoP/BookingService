using Booking.Application.Abstractions;

namespace Booking.Application.UseCases.Bookings.CancelBooking
{
    public sealed record CancelBookingCommand(Guid bookingId, string UserId) : ICommand<Guid>;
}
