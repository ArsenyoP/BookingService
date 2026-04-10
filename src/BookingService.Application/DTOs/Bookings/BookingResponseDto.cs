namespace Booking.Application.DTOs.Bookings
{
    public record BookingResponseDto(Guid Id,
        Guid RoomId,
        Guid GuestId,
        DateOnly StartDate,
        DateOnly EndDate,
        int TotalNights,
        decimal PricePerNight,
        decimal TotalPrice,
        int AdultsCount,
        int ChildrenCount,
        string Status);
}
