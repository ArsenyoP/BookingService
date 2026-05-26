using Booking.Application.Abstractions;

namespace Booking.Application.UseCases.Users.InvalidateRefreshToken
{
    public sealed record InvalidateRefreshTokenCommand(Guid userId) : ICommand<bool>;
}
