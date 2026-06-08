using Booking.Domain.Common;

namespace Booking.Domain.DomainEvents
{
    public sealed record RoomCreatedDomainEvent(Guid roomId,
        double pricePerNight,
        string city,
        string searchText) : IDomainEvent
    {
    }
}
