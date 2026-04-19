using Booking.Application.Abstractions;

namespace Booking.Application.UseCases.Amenities.RemoveAmenityFromListing
{
    public sealed record RemoveAmenityFromListingCommand(Guid listingId, string amenityName) : ICommand<Guid>;
}
