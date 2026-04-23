using Booking.Application.DTOs.Users;
using Booking.Domain.Common;
using Booking.Domain.Enums;

namespace Booking.Application.Interfaces.Services
{
    public interface IAuthService
    {
        public Task<Result<UserDto>> RegisterUser(RegisterDto registerDto, UserRole role = UserRole.Guest, CancellationToken ct = default);
    }
}
