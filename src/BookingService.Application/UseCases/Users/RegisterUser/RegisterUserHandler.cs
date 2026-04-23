using Booking.Application.Abstractions;
using Booking.Application.DTOs.Users;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Common;

namespace Booking.Application.UseCases.Users.RegisterUser
{
    public sealed class RegisterUserHandler(IAuthService _authService) : ICommandHandler<RegisterUserCommand, UserDto>
    {
        public async Task<Result<UserDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var registerDto = request.registerDto;

            var result = await _authService.RegisterUser(registerDto, request.role);
            return result;
        }
    }
}
