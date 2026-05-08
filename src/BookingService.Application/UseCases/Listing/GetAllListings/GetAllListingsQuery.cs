using Booking.Application.Abstractions;
using Booking.Application.DTOs.Listings;

namespace Booking.Application.UseCases.Listing.GetAllListings
{
    public sealed record GetAllListingsQuery(int Page, int PageSize, List<string>? AmenityNames = null) : IQuery<IReadOnlyList<ListingResponseDto>>;
}
