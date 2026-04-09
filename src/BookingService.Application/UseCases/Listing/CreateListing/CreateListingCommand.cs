using Booking.Application.Abstractions;
using Booking.Domain.Enums;
using Booking.Domain.ValueObjects;


namespace Booking.Application.UseCases.Listing.CreateListing
{
    public sealed record CreateListingCommand(
        string Title,
        string Description,
        string Country,
        string City,
        string Street,
        string HouseNumber,
        int Floor,
        ListingType ListingType) : ICommand<Guid>;
}
