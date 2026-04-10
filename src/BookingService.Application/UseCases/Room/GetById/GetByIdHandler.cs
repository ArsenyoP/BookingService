using Booking.Application.Abstractions;
using Booking.Application.DTOs;
using Booking.Application.DTOs.Rooms;
using Booking.Application.Queries;
using Booking.Domain.Common;
using Booking.Domain.Errors;

namespace Booking.Application.UseCases.Room.GetById
{
    public class GetByIdHandler(IRoomQueries _roomQueries) : IQueryHandler<GetByIdQuery, RoomResponseDto>
    {
        public async Task<Result<RoomResponseDto>> Handle(GetByIdQuery request, CancellationToken ct)
        {
            var room = await _roomQueries.GetByIdAsync(request.Id, ct);

            if (room is null)
            {
                return Result<RoomResponseDto>.Failure(RoomErrors.NotFound);
            }

            return Result<RoomResponseDto>.Success(room);
        }
    }
}
