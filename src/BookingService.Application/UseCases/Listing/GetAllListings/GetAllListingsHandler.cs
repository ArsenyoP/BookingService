using Booking.Application.Abstractions;
using Booking.Application.DTOs.Listings;
using Booking.Domain.Common;
using Booking.Domain.Interfaces.IRepositories;

namespace Booking.Application.UseCases.Listing.GetAllListings
{
    internal class GetAllListingsHandler(IListingRepository listingRepository)
        : IQueryHandler<GetAllListingsQuery, List<ListingResponseDto>>
    {
        public async Task<Result<List<ListingResponseDto>>> Handle(GetAllListingsQuery request, CancellationToken ct)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var listings = await listingRepository.GetPagedAsync(page, pageSize, ct);

            var response = listings.Select(l => new ListingResponseDto(
                l.Id,
                l.Title,
                l.Description,
                l.Address.Country,
                l.Address.City,
                l.Address.Street,
                l.Address.HouseNumber,
                l.Address.Floor,
                l.ListingType
            )).ToList();

            return Result<List<ListingResponseDto>>.Success(response);
        }
    }
}
