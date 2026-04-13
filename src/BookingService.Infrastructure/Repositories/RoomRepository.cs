using Booking.Domain.Entities;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Infrastructure.Data;

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
    }
}
