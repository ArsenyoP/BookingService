using Booking.Application.Abstractions;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Common;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Domain.Errors;
using Booking.Application.Interfaces;

namespace Booking.Application.UseCases.Amenities.AddAmenityToListing
{
    public sealed class AddAmenityToListingHandler(IListingRepository _listingRepository,
        IAmenityQueries _amenityQueries, IUnitOfWork _unitOfWork) : ICommandHandler<AddAmenityToListingCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(AddAmenityToListingCommand request, CancellationToken ct)
        {
            var listing = await _listingRepository.GetByIdWithAmenities(request.ListingId, ct);
            if (listing is null)
                return Result<Guid>.Failure(ListingErrors.NotFound);

            var amenity = await _amenityQueries.GetByNameAsync(request.AmenityName, ct);
            if (amenity is null)
                return Result<Guid>.Failure(AmenityErrors.NotFound);

            var result = listing.AddAmenity(amenity);

            if (!result.IsSuccess)
            {
                return Result<Guid>.Failure(result.Error);
            }

            await _unitOfWork.SaveChangesAsync(ct);
            return Result<Guid>.Success(amenity.Id);
        }
    }
}
