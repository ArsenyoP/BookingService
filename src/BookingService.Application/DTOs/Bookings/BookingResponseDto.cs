namespace Booking.Application.DTOs.Bookings
{
    public record BookingResponseDto(Guid Id,
        Guid RoomId,
        Guid GuestId,
        DateTime StartDate,
        DateTime EndDate,
        int TotalNights,
        decimal PricePerNight,
        decimal TotalPrice,
        int AdultsCount,
        int ChildrenCount,
        string Status,
        string RoomTitle,
        string FirstName,
        string LastName);
}
