namespace Booking.Application.DTOs.Users
{
    public sealed record RegisterDto(string UserName,
        string FirstName,
        string LastName,
        string Email,
        string Password,
        DateOnly DateOfBirth);
}
