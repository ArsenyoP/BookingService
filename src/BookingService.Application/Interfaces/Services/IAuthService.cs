using Booking.Application.DTOs.Users;
using Booking.Domain.Common;

namespace Booking.Application.Interfaces.Services
{
    public interface IAuthService
    {
        public Task<Result<AuthResult>> RegisterUser(RegisterDto registerDto, string role = "Guest", CancellationToken ct = default);
        public Task<Result<AuthResult>> LoginUser(LoginDto loginDto, CancellationToken ct = default);
    }
}
