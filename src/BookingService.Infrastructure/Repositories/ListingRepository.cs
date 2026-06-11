using Booking.Domain.Entities;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;

namespace Booking.Infrastructure.Repositories
{
    public class ListingRepository(AppDbContext _dbContext) : IListingRepository
    {
        public void Add(Listing listing)
        {
            ArgumentNullException.ThrowIfNull(listing);
            _dbContext.Listings.Add(listing);
        }

        public void Delete(Listing listing)
        {
            ArgumentNullException.ThrowIfNull(listing);
            _dbContext.Listings.Remove(listing);
        }

        public async Task<List<Listing?>> GetByIds(IEnumerable<Guid> ids, CancellationToken ct = default)
        {
            var listings = await _dbContext.Listings
               .Where(r => ids.Contains(r.Id))
               .Include(r => r.Amenities)
               .ToListAsync();

            return listings;
        }

        public async Task<Listing?> GetByIdWithAmenities(Guid id, CancellationToken ct = default)
        {
            var result = await _dbContext.Listings.Include(l => l.Amenities).FirstOrDefaultAsync(l => l.Id == id);

            return result;
        }
    }
}
