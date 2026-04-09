using Booking.Application.DTOs.Rooms;
using Booking.Domain.Entities;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Dapper;

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

        public async Task<Room?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await dbContext.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id, ct);
        }

        public Task<IReadOnlyList<Room>> GetByListingIdAsync(Guid listingId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
