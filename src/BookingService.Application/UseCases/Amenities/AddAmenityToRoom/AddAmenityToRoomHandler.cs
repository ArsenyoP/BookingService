using Booking.Application.Abstractions;
using Booking.Application.Interfaces;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Common;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;

namespace Booking.Application.UseCases.Amenities.AddAmenityToRoom
{
    public sealed class AddAmenityToRoomHandler(IAmenityQueries _amenityQueries,
        IRoomRepository _roomRepository, IUnitOfWork _unitOfWork) : ICommandHandler<AddAmenityToRoomCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(AddAmenityToRoomCommand request, CancellationToken ct)
        {
            var amenity = await _amenityQueries.GetByNameAsync(request.AmenityName, ct);
            if (amenity is null)
                return Result<Guid>.Failure(AmenityErrors.NotFound);

            var room = await _roomRepository.GetByIdWithAmenities(request.RoomId, ct);
            if (room is null)
                return Result<Guid>.Failure(RoomErrors.NotFound);

            var result = room.AddAmentity(amenity);


            if (!result.IsSuccess)
            {
                return Result<Guid>.Failure(result.Error);
            }

            await _unitOfWork.SaveChangesAsync(ct);

            return Result<Guid>.Success(amenity.Id);


        }
    }
}
