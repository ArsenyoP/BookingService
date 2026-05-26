using Booking.Application.Abstractions;

namespace Booking.Application.UseCases.Users.LoginWithRefreshToken
{
    public sealed record LoginWithRefreshTokenResponse(string AccessToken, string RefreshToken);
    public sealed record LoginWithRefreshTokenCommand(string RefreshToken) : ICommand<LoginWithRefreshTokenResponse>;
}
