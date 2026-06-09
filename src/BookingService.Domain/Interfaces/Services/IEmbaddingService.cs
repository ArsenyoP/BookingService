using Booking.Domain.DomainEvents;


namespace Booking.Domain.Interfaces.Services
{
    public interface IEmbaddingService
    {
        Task EmbaddeEvent(RoomCreatedDomainEvent domainEvent, CancellationToken ct = default);
    }
}
