using Booking.Application.Abstractions;
using Booking.Application.DTOs.Rooms;
using Booking.Application.Helpers.Room;

namespace Booking.Application.UseCases.Room.GetAllRooms
{
    public sealed record GetAllRoomsQuery(RoomQueryObject QueryObject) : IQuery<IReadOnlyList<RoomResponseDto>>;
}
