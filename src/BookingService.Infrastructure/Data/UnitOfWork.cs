using Booking.Application.Interfaces;

namespace Booking.Infrastructure.Data
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;

        public UnitOfWork(AppDbContext dbContect)
        {
            _dbContext = dbContect;
        }
        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
            => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
