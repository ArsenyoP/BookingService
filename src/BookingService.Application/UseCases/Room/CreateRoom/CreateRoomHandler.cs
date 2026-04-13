using Booking.Application.Abstractions;
using Booking.Domain.Common;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Domain.Errors;
using Booking.Application.Interfaces;
using Booking.Application.Interfaces.IQueries;

namespace Booking.Application.UseCases.Room.CreateRoom;

public class CreateRoomHandler(IRoomRepository _roomRepository, IListingQueries _listingQueries,
    IUnitOfWork _unitOfWork) : ICommandHandler<CreateRoomCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateRoomCommand request, CancellationToken ct)
    {
        var listing = await _listingQueries.GetEntityByIdAsync(request.ListingId);

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
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(roomResult.Value.Id);
    }
}
