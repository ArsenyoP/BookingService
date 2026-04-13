namespace Booking.Application.Interfaces
{
    public interface IUnitOfWork
    {
        public Task SaveChangesAsync(CancellationToken ct = default);
    }
}
