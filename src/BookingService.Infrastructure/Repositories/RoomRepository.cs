using Booking.Domain.Entities;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories
{
    public class RoomRepository(AppDbContext dbContext) : IRoomRepository
    {
        public void Add(Room obj)
        {
            ArgumentNullException.ThrowIfNull(obj);
            dbContext.Rooms.Add(obj);
        }

        public void Delete(Room obj)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken ct = default)
        {
            return await dbContext.Rooms
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Room>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            return await dbContext.Rooms
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public async Task<Room?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await dbContext.Rooms
                .FirstOrDefaultAsync(r => r.Id == id, ct);
        }

        public Task<IReadOnlyList<Room>> GetByListingIdAsync(Guid listingId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
