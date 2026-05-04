using Booking.Application.Abstractions;
using Booking.Application.DTOs.Rooms;


namespace Booking.Application.UseCases.Room.GetById
{
    public sealed record GetByIdQuery(Guid Id) : IQuery<RoomResponseDto>, ICachableQuery
    {
        public string Key => $"room:{Id}";

        public TimeSpan Expiration => TimeSpan.FromMinutes(5);
    }
}
