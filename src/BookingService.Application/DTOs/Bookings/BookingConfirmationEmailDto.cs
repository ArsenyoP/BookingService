namespace Booking.Application.DTOs.Bookings
{
    public sealed record BookingConfirmationEmailDto(
        string GuestEmail,
        string GuestName,
        string RoomTitle,
        DateTime StartDate,
        DateTime EndDate,
        decimal TotalPrice);
}
