using Booking.Application.Abstractions;
using Booking.Application.DTOs;
using Booking.Application.DTOs.Rooms;
using Booking.Application.Mappers;
using Booking.Domain.Common;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;

namespace Booking.Application.UseCases.Room.GetById
{
    public class GetByIdHandler(IRoomRepository _roomRepository) : IQueryHandler<GetByIdQuery, RoomResponseDto>
    {
        public async Task<Result<RoomResponseDto>> Handle(GetByIdQuery request, CancellationToken ct)
        {
            var room = await _roomRepository.GetByIdAsync(request.Id, ct);

            if (room is null)
            {
                return Result<RoomResponseDto>.Failure(RoomErrors.NotFound);
            }

            var response = room.FromRoomToRoomResponseDto();

            return Result<RoomResponseDto>.Success(response);
        }
    }
}
