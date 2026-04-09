using Booking.Application.Abstractions;
using Booking.Application.DTOs.Listings;

namespace Booking.Application.UseCases.Listing.GetAllListings
{
    public sealed record GetAllListingsQuery(int Page, int PageSize) : IQuery<List<ListingResponseDto>>;
}
