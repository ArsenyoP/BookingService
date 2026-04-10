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
    }
}
