using Booking.Domain.Entities;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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

        public async Task<RefreshToken?> GetById(string refreshToken)
        {
            var token = await _dbContext.RefreshToken
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            return token;
        }
    }
}
