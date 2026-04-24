using Booking.Application.DTOs.Users;
using System.Windows.Input;
using Booking.Application.Abstractions;
using Booking.Domain.Enums;

namespace Booking.Application.UseCases.Users.RegisterUser
{
    public sealed record RegisterUserCommand(RegisterDto registerDto, string role) : ICommand<UserDto>;
}
