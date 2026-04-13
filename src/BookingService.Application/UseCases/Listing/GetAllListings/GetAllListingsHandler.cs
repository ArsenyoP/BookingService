using Booking.Application.Abstractions;
using Booking.Application.DTOs.Listings;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Common;

namespace Booking.Application.UseCases.Listing.GetAllListings
{
    public class GetAllListingsHandler(IListingQueries _listingQueries)
        : IQueryHandler<GetAllListingsQuery, IReadOnlyList<ListingResponseDto>>
    {
        public async Task<Result<IReadOnlyList<ListingResponseDto>>> Handle(GetAllListingsQuery request, CancellationToken ct)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var listings = await _listingQueries.GetAllPagedAsync(page, pageSize, ct);

            return Result<IReadOnlyList<ListingResponseDto>>.Success(listings);
        }
    }
}
