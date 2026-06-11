using Booking.Domain.Common;

namespace Booking.Domain.DomainEvents
{
    internal class LisitngCreatedDomainEvent(Guid listingId,
        string city,
        string street,
        string searchText) : IDomainEvent;
}
