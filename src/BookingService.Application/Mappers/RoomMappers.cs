using Booking.Application.DTOs.Rooms;
using Booking.Domain.Entities;

namespace Booking.Application.Mappers
{
    public static class RoomMappers
    {
        public static RoomResponseDto FromRoomToRoomResponseDto(this Room room)
        {
            return new RoomResponseDto(
                room.Id,
                room.Title,
                room.Description,
                room.Type,
                room.PricePerNight,
                room.AdultsCapacity,
                room.ChildrenCapacity,
                room.ListingId
            );
        }

    }
}
