using Booking.Application.Abstractions;
using Booking.Application.Interfaces;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Common;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;

namespace Booking.Application.UseCases.Amenities.RemoveAmenityFromListing
{
    public sealed class RemoveAmenityFromListingHandler(
    IListingRepository _listingRepository,
    IAmenityQueries _amenityQueries,
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveAmenityFromListingCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(RemoveAmenityFromListingCommand request, CancellationToken ct)
        {
            var listing = await _listingRepository.GetByIdWithAmenities(request.listingId, ct);
            if (listing is null)
            {
                return Result<Guid>.Failure(ListingErrors.NotFound);
            }

            var amenity = await _amenityQueries.GetByNameAsync(request.amenityName, ct);
            if (amenity is null)
            {
                return Result<Guid>.Failure(AmenityErrors.NotFound);
            }

            var result = listing.RemoveAmenity(amenity);

            if (!result.IsSuccess)
            {
                return Result<Guid>.Failure(result.Error);
            }

            await unitOfWork.SaveChangesAsync(ct);
            return Result<Guid>.Success(amenity.Id);
        }
    }
}
