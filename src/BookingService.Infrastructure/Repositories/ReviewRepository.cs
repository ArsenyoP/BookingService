using Booking.Domain.Entities;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
            _dbContext.Attach(obj);
            _dbContext.Remove(obj);
        }

        public async Task<Review?> GetReviewByUserIdAndTargetId(Guid userId, Guid targetId, CancellationToken ct)
        {
            return await _dbContext.Review.FirstOrDefaultAsync(x => x.UserId == userId && x.TargetId == targetId);
        }
    }
}
