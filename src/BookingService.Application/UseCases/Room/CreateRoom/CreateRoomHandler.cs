using Booking.Application.Abstractions;
using Booking.Application.Interfaces;
using Booking.Domain.Common;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;
using RoomEntity = Booking.Domain.Entities.Room;


namespace Booking.Application.UseCases.Room.CreateRoom;

public class CreateRoomHandler(IRoomRepository _roomRepository, IListingRepository _listingRepository,
    IUnitOfWork _unitOfWork) : ICommandHandler<CreateRoomCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateRoomCommand request, CancellationToken ct)
    {
        var listing = await _listingRepository.GetByIdWithAmenities(request.ListingId);

        if (listing is null)
        {
            return Result<Guid>.Failure(ListingErrors.NotFound);
        }

        var roomResult = RoomEntity.Create(
            request.Title,
            request.Description,
            request.Type,
            request.PricePerNight,
            request.AdultsCapacity,
            request.ChildrenCapacity,
            listing.Id,
            listing.Address.City);

        if (!roomResult.IsSuccess || roomResult.Value is null)
        {
            return Result<Guid>.Failure(roomResult.Error);
        }

        _roomRepository.Add(roomResult.Value);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(roomResult.Value.Id);
    }
}
