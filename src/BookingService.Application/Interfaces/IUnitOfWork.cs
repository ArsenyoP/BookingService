namespace Booking.Application.Interfaces
{
    public interface IUnitOfWork
    {
        public Task SaveChangesAsync(CancellationToken ct = default);
        Task<T> ExecuteInSerializableTransactionAsync<T>(
            Func<Task<T>> operation,
            CancellationToken ct = default);
    }
}
