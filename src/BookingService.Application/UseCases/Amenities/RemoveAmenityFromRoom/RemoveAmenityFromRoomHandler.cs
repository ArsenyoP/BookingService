using Booking.Application.Abstractions;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Common;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Domain.Errors;
using Booking.Application.Interfaces;

namespace Booking.Application.UseCases.Amenities.RemoveAmenityFromRoom
{
    public sealed class RemoveAmenityFromRoomHandler(IRoomRepository _roomRepository,
        IAmenityQueries _amenityQueries, IUnitOfWork unitOfWork) : ICommandHandler<RemoveAmenityFromRoomCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(RemoveAmenityFromRoomCommand request, CancellationToken ct)
        {
            var room = await _roomRepository.GetByIdWithAmenities(request.roomId, ct);
            if (room is null)
            {
                return Result<Guid>.Failure(RoomErrors.NotFound);
            }

            var amenity = await _amenityQueries.GetByNameAsync(request.amenityName, ct);
            if (amenity is null)
            {
                return Result<Guid>.Failure(AmenityErrors.NotFound);
            }

            var result = room.RemoveAmenity(amenity);

            if (!result.IsSuccess)
            {
                return Result<Guid>.Failure(result.Error);
            }

            await unitOfWork.SaveChangesAsync(ct);
            return Result<Guid>.Success(amenity.Id);
        }
    }
}
