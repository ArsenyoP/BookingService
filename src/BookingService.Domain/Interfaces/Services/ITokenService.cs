using Booking.Domain.Entities;

namespace Booking.Domain.Interfaces.Services
{
    public interface ITokenService
    {
        public Task<string> CreateToken(User user);
    }
}
