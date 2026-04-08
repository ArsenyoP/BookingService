using Booking.Application.Abstractions;
using Booking.Domain.Common;
using Booking.Domain.Entities;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Domain.Errors;
using Booking.Application.Interfaces;

namespace Booking.Application.UseCases.Room.CreateRoom;

public class CreateRoomHandler(IListingRepository _listingRepository, IRoomRepository _roomRepository,
    IUnitOfWork _unitOfWork) : ICommandHandler<CreateRoomCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var listing = await _listingRepository.GetByIdAsync(request.ListingId);

        if (listing is null)
        {
            return Result<Guid>.Failure(ListingErrors.NotFound);
        }

        var roomResult = Booking.Domain.Entities.Room.Create(
            request.Title,
            request.Description,
            request.Type,
            request.PricePerNight,
            request.AdultsCapacity,
            request.ChildrenCapacity,
            listing.Id);

        if (!roomResult.IsSuccess || roomResult.Value is null)
        {
            return Result<Guid>.Failure(roomResult.Error);
        }

        _roomRepository.Add(roomResult.Value);
        await _unitOfWork.SaveChangesAsync();

        return Result<Guid>.Success(roomResult.Value.Id);
    }
}
