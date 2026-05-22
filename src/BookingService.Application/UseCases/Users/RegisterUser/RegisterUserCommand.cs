using Booking.Application.Abstractions;
using Booking.Application.DTOs.Users;

namespace Booking.Application.UseCases.Users.RegisterUser
{
    public sealed record RegisterUserCommand(RegisterDto registerDto, string role) : ICommand<AuthResult>;
}
