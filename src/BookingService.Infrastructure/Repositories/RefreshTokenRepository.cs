using Booking.Domain.Entities;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Infrastructure.Data;

namespace Booking.Infrastructure.Repositories
{
    public sealed class RefreshTokenRepository(AppDbContext _dbContext) : IRefreshTokenRepository
    {
        public void Add(RefreshToken refreshToken)
        {
            ArgumentNullException.ThrowIfNull(refreshToken);
            _dbContext.Add(refreshToken);
        }

        public void Delete(RefreshToken refreshToken)
        {
            ArgumentNullException.ThrowIfNull(refreshToken);
            _dbContext.Remove(refreshToken);
        }
    }
}
