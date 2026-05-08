using Booking.Application.Abstractions;
using Booking.Application.DTOs.Rooms;
using Booking.Application.Queries;
using Booking.Domain.Common;

namespace Booking.Application.UseCases.Room.GetAllRooms
{
    internal class GetAllRoomsHandler(IRoomQueries _roomQueries)
        : IQueryHandler<GetAllRoomsQuery, IReadOnlyList<RoomResponseDto>>
    {
        public async Task<Result<IReadOnlyList<RoomResponseDto>>> Handle(GetAllRoomsQuery request, CancellationToken ct)
        {
            var page = request.QueryObject.Page < 1 ? 1 : request.QueryObject.Page;
            var pageSize = request.QueryObject.PageSize < 1 ? 10 : request.QueryObject.PageSize;

            var rooms = await _roomQueries.GetAllWithAmenitiesAsync(request.QueryObject, ct);

            return Result<IReadOnlyList<RoomResponseDto>>.Success(rooms);
        }
    }
}
