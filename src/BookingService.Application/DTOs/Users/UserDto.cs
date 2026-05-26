namespace Booking.Application.DTOs.Users
{
    public sealed record UserDto(
        string UserName,
        string FirstName,
        string LastName,
        string Email,
        string JwtToken,
        string RefreshToke);

    public sealed record AuthResult(UserDto UserDto, string RefreshToken);
}
