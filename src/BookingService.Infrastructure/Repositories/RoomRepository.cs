using Booking.Domain.Entities;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories
{
    public class RoomRepository(AppDbContext dbContext) : IRoomRepository
    {
        public void Add(Room room)
        {
            ArgumentNullException.ThrowIfNull(room);
            dbContext.Rooms.Add(room);
        }

        public void Delete(Room room)
        {
            ArgumentNullException.ThrowIfNull(room);
            dbContext.Rooms.Remove(room);
        }

        public async Task<List<Room?>> GetByIds(IEnumerable<Guid> ids, CancellationToken ct = default)
        {
            var rooms = await dbContext.Rooms
                .Where(r => ids.Contains(r.Id))
                .Include(r => r.Amenities)
                .ToListAsync();

            return rooms;
        }

        public async Task<Room?> GetByIdWithAmenities(Guid Id, CancellationToken ct = default)
        {
            var result = await dbContext.Rooms
                .Include(r => r.Amenities)
                .Include("_listing")
                .FirstOrDefaultAsync(r => r.Id == Id);

            return result;
        }
    }
}
