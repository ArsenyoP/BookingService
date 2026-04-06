using Booking.Application.Abstractions;

namespace Booking.Application.UseCases.Bookings.CreateBooking
{
    public sealed record CreateBookingCommand(
        Guid RoomId,
        Guid GuestId,
        DateOnly StartDate,
        DateOnly EndDate,
        int AdultsCount,
        int ChildrenCount) : ICommand<Guid>;
}
