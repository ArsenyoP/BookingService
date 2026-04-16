using Booking.Application.Abstractions;

namespace Booking.Application.UseCases.Amenities.AddAmenityToListing
{
    public sealed record AddAmenityToListingCommand(Guid ListingId, string AmenityName) : ICommand<Guid>;
}
