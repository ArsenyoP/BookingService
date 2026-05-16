namespace Booking.Application.DTOs.Bookings
{
    public sealed record CreateBookingDto(Guid RoomId,
        DateOnly StartDate,
        DateOnly EndDate,
        int AdultsCount,
        int ChildrenCount);
}
