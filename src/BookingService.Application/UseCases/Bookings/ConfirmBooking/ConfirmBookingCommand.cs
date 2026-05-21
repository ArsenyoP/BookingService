using Booking.Application.Abstractions;

namespace Booking.Application.UseCases.Bookings.ConfirmBooking
{
    public sealed record ConfirmBookingCommand(string confirmationToken) : ICommand<Guid>;
}
