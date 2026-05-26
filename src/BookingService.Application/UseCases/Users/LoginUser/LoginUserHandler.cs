using Booking.Application.Abstractions;
using Booking.Application.DTOs.Users;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Common;

namespace Booking.Application.UseCases.Users.LoginUser
{
    public sealed class LoginUserHandler(IAuthService _authService) : ICommandHandler<LoginUserCommand, AuthResult>
    {
        public async Task<Result<AuthResult>> Handle(LoginUserCommand request, CancellationToken ct)
        {
            var result = await _authService.LoginUser(request.LoginDto, ct);
            return result;
        }
    }
}
