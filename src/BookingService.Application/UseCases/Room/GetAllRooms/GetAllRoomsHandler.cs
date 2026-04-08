using Booking.Application.Abstractions;
using Booking.Application.DTOs.Rooms;
using Booking.Domain.Common;
using Booking.Domain.Interfaces.IRepositories;

namespace Booking.Application.UseCases.Room.GetAllRooms
{
    internal class GetAllRoomsHandler(IRoomRepository _roomRepository)
        : IQueryHandler<GetAllRoomsQuery, List<RoomResponseDto>>
    {
        public async Task<Result<List<RoomResponseDto>>> Handle(GetAllRoomsQuery request, CancellationToken ct)
        {
            //TODO: Add validation throught FluentValidation
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var rooms = await _roomRepository.GetPagedAsync(page, pageSize, ct);

            var response = rooms.Select(r => new RoomResponseDto(
                r.Id,
                r.Title,
                r.Description,
                r.Type,
                r.PricePerNight,
                r.AdultsCapacity,
                r.ChildrenCapacity,
                r.ListingId
            )).ToList();

            return Result<List<RoomResponseDto>>.Success(response);
        }
    }
}
