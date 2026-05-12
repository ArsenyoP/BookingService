using Booking.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Booking.Infrastructure.Data
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private IDbContextTransaction? _currentTransaction;

        public UnitOfWork(AppDbContext dbContect)
        {
            _dbContext = dbContect;
        }

        public async Task<IDisposable> BeginTransactionAsync(CancellationToken ct = default)
        {
            if (_currentTransaction is not null)
            {
                return _currentTransaction;
            }

            _currentTransaction = await _dbContext.Database.BeginTransactionAsync(ct);
            return _currentTransaction;
        }

        public async Task CommitAsync(CancellationToken ct = default)
        {
            if (_currentTransaction is null)
            {
                return;
            }

            try
            {
                await _currentTransaction.CommitAsync(ct);
            }
            finally
            {
                DisposeTransaction();
            }
        }

        public async Task RollbackAsync(CancellationToken ct = default)
        {
            if (_currentTransaction is not null)
            {
                await _currentTransaction.RollbackAsync(ct);
                DisposeTransaction();
            }
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

        public IDbTransaction? GetCurrentTransaction()
        {
            return _currentTransaction?.GetDbTransaction();
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
            => _dbContext.SaveChangesAsync(cancellationToken);

        private void DisposeTransaction()
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }
}
