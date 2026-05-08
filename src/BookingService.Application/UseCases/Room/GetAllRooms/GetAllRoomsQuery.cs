using Booking.Application.Abstractions;
using Booking.Application.DTOs.Rooms;

namespace Booking.Application.UseCases.Room.GetAllRooms
{
    public sealed record GetAllRoomsQuery(int Page, int PageSize, List<string>? AmenityNames = null) : IQuery<IReadOnlyList<RoomResponseDto>>;
}
