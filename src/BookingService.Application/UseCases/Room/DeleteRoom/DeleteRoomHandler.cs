using Booking.Application.Abstractions;
using Booking.Application.Interfaces;
using Booking.Application.Queries;
using Booking.Domain.Common;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Domain.Errors;

namespace Booking.Application.UseCases.Room.DeleteRoom
{
    public sealed class DeleteRoomHandler(IRoomRepository _roomRepository, IRoomQueries _roomQueries,
        IUnitOfWork _unitOfWork) : ICommandHandler<DeleteRoomCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(DeleteRoomCommand request, CancellationToken ct)
        {
            var room = await _roomQueries.GetEntityByIdAsync(request.roomId, ct);
            if (room is null)
            {
                return Result<Guid>.Failure(RoomErrors.NotFound);
            }

            _roomRepository.Delete(room);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<Guid>.Success(room.Id);
        }
    }
}
