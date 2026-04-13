using Booking.Application.Abstractions;
using Booking.Application.DTOs.Listings;

namespace Booking.Application.UseCases.Listing.GetById
{
    public sealed record GetByIdQuery(Guid id) : IQuery<ListingResponseDto>;

}
