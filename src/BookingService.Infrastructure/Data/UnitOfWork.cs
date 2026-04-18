using Booking.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Data
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;

        public UnitOfWork(AppDbContext dbContect)
        {
            _dbContext = dbContect;
        }

        public async Task<T> ExecuteInSerializableTransactionAsync<T>(Func<Task<T>> operation, CancellationToken ct = default)
        {
            await _dbContext.Database.OpenConnectionAsync(ct);

            var strategy = _dbContext.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _dbContext.Database.BeginTransactionAsync(
                    System.Data.IsolationLevel.Serializable, ct);

                try
                {
                    var result = await operation();
                    await _dbContext.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync(ct);
                    throw;
                }
            });
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
            => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
