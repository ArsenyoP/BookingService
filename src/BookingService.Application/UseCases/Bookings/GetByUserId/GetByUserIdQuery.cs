using Booking.Application.Abstractions;
using Booking.Application.DTOs.Bookings;


namespace Booking.Application.UseCases.Bookings.GetByUserId
{
    public sealed record GetByUserIdQuery(Guid userId, int page, int pageSize) : IQuery<IReadOnlyList<BookingResponseDto>>;
}
