using Booking.Domain.Entities;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Infrastructure.Data;

namespace Booking.Infrastructure.Repositories
{
    public class AmenityRepository(AppDbContext _dbContext) : IAmenityRepository
    {
        public void Add(Amenity amenity)
        {
            ArgumentNullException.ThrowIfNull(amenity);
            _dbContext.Add(amenity);
        }

        public void Delete(Amenity amenity)
        {
            ArgumentNullException.ThrowIfNull(amenity);
            _dbContext.Remove(amenity);
        }
    }
}
