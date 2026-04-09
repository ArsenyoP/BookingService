using Booking.Application.Abstractions;
using Booking.Application.DTOs.Rooms;


namespace Booking.Application.UseCases.Room.GetById
{
    public sealed record GetByIdQuery(Guid Id) : IQuery<RoomResponseDto>;
}
