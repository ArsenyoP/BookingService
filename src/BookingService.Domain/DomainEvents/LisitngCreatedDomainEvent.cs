using Booking.Domain.Common;

namespace Booking.Domain.DomainEvents
{
    public sealed record LisitngCreatedDomainEvent(Guid listingId,
        string city,
        string street,
        string searchText) : IDomainEvent;
}
