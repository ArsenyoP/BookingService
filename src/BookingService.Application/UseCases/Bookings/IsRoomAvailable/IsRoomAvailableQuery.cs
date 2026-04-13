using Booking.Application.Abstractions;


namespace Booking.Application.UseCases.Bookings.IsRoomAvailable
{
    public sealed record IsRoomAvailableQuery(Guid roomId, DateOnly start, DateOnly end) : IQuery<bool>;
}
