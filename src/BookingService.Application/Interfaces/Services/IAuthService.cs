using Booking.Application.DTOs.Users;
using Booking.Domain.Common;
using Booking.Domain.Enums;

namespace Booking.Application.Interfaces.Services
{
    public interface IAuthService
    {
        public Task<Result<UserDto>> RegisterUser(RegisterDto registerDto, string role = "Guest", CancellationToken ct = default);
        public Task<Result<UserDto>> LoginUser(LoginDto loginDto, CancellationToken ct = default);
    }
}
