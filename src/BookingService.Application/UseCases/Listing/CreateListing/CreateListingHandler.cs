using Booking.Application.Abstractions;
using Booking.Application.Interfaces;
using Booking.Domain.Common;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Domain.ValueObjects;

namespace Booking.Application.UseCases.Listing.CreateListing;

public class CreateListingHandler(IListingRepository _listingRepository,
    IUnitOfWork _unitOfWork) : ICommandHandler<CreateListingCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateListingCommand request, CancellationToken ct)
    {
        var addressResult = Address.Create(request.Country,
            request.City, request.Street, request.HouseNumber, request.Floor);

        if (!addressResult.IsSuccess || addressResult.Value is null)
        {
            return Result<Guid>.Failure(addressResult.Error);
        }

        var listingResult = Booking.Domain.Entities.Listing.Create(
            request.Title,
            request.Description,
            addressResult.Value,
            request.ListingType);

        if (!listingResult.IsSuccess || listingResult.Value is null)
        {
            return Result<Guid>.Failure(listingResult.Error);
        }

        _listingRepository.Add(listingResult.Value);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(listingResult.Value.Id);
    }
}
