using Booking.Domain.Entities;

namespace Booking.Domain.Interfaces.IRepositories
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
    {
        public Task<RefreshToken?> GetById(string refreshToken);
        public Task<bool> InvalidateUsersToken(Guid userId);
        public Task<bool> CleanExpiredTokens(CancellationToken ct = default);
    }
}
