using Booking.Domain.Entities;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Listing?> GetByIdWithAmenities(Guid id, CancellationToken ct = default)
        {
            var result = await _dbContext.Listings.Include(l => l.Amenities).FirstOrDefaultAsync(l => l.Id == id);

            return result;
        }
    }
}
