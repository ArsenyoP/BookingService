using Booking.Application.Abstractions;
using Booking.Application.Interfaces;
using Booking.Application.Interfaces.IQueries;
using Booking.Application.Queries; // Припускаю, що тут лежать IListingQueries
using Booking.Domain.Common;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;

namespace Booking.Application.UseCases.Listing.DeleteListing
{
    public sealed class DeleteListingHandler(
        IListingRepository _listingRepository,
        IListingQueries _listingQueries,
        IUnitOfWork _unitOfWork) : ICommandHandler<DeleteListingCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(DeleteListingCommand request, CancellationToken ct)
        {
            var listing = await _listingQueries.GetEntityByIdAsync(request.ListingId, ct);

            if (listing is null)
            {
                return Result<Guid>.Failure(ListingErrors.NotFound);
            }

            _listingRepository.Delete(listing);

            await _unitOfWork.SaveChangesAsync(ct);
            return Result<Guid>.Success(listing.Id);
        }
    }
}