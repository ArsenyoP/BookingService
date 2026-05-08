using Booking.Application.Abstractions;
using Booking.Application.DTOs.Listings;
using Booking.Application.Helpers.Room;

namespace Booking.Application.UseCases.Listing.GetAllListings
{
    public sealed record GetAllListingsQuery(ListingQueryObject QueryObject) : IQuery<IReadOnlyList<ListingResponseDto>>;
}
