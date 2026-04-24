using Booking.Application.Abstractions;
using Booking.Application.DTOs.Users;

namespace Booking.Application.UseCases.Users.LoginUser
{
    public sealed record LoginUserCommand(LoginDto LoginDto) : ICommand<UserDto>;
}
