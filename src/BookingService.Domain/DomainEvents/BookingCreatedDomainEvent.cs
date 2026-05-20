using Booking.Domain.Common;

namespace Booking.Domain.DomainEvents
{
    public sealed record BookingCreatedDomainEvent(Guid BookingId) : IDomainEvent
    {
    }
}
