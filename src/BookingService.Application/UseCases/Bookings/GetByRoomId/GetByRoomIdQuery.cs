using Booking.Application.Abstractions;
using Booking.Application.DTOs.Bookings;


namespace Booking.Application.UseCases.Bookings.GetByRoomId
{
    public sealed record GetByRoomIdQuery(Guid roomId, int page, int pageSize) : IQuery<IReadOnlyList<BookingResponseDto>>;
}
