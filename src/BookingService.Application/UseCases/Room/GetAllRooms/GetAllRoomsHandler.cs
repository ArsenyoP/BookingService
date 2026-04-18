using Booking.Application.Abstractions;
using Booking.Application.DTOs.Rooms;
using Booking.Application.Queries;
using Booking.Domain.Common;
using Booking.Domain.Interfaces.IRepositories;

namespace Booking.Application.UseCases.Room.GetAllRooms
{
    internal class GetAllRoomsHandler(IRoomQueries _roomQueries)
        : IQueryHandler<GetAllRoomsQuery, IReadOnlyList<RoomResponseDto>>
    {
        public async Task<Result<IReadOnlyList<RoomResponseDto>>> Handle(GetAllRoomsQuery request, CancellationToken ct)
        {
            //TODO: Add validation throught FluentValidation
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var rooms = await _roomQueries.GetAllWithAmenitiesAsync(page, pageSize, ct: ct);

            return Result<IReadOnlyList<RoomResponseDto>>.Success(rooms);
        }
    }
}
