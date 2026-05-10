using Booking.Domain.Entities;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Infrastructure.Data;

namespace Booking.Infrastructure.Repositories
{
    public sealed class ReviewRepository(AppDbContext _dbContext) : IReviewRepository
    {
        public async void Add(Review obj)
        {
            ArgumentNullException.ThrowIfNull(obj);
            await _dbContext.AddAsync(obj);
        }

        public void Delete(Review obj)
        {
            ArgumentNullException.ThrowIfNull(obj);
            _dbContext.Remove(obj);
        }
    }
}
