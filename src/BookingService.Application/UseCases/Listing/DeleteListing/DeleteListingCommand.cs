using Booking.Application.Abstractions;

namespace Booking.Application.UseCases.Listing.DeleteListing
{
    public sealed record DeleteListingCommand(Guid ListingId) : ICommand<Guid>, ICacheInvalidationCommand
    {
        public string Key => $"listing:{ListingId}";
    }
}
