using System.Data;

namespace Booking.Application.Interfaces
{
    public interface IUnitOfWork
    {
        public Task SaveChangesAsync(CancellationToken ct = default);

        Task<IDisposable> BeginTransactionAsync(CancellationToken ct = default);

        Task CommitAsync(CancellationToken ct = default);

        Task RollbackAsync(CancellationToken ct = default);

        IDbTransaction? GetCurrentTransaction();

        Task<T> ExecuteInSerializableTransactionAsync<T>(
            Func<Task<T>> operation,
            CancellationToken ct = default);
    }
}
