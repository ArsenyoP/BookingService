using Booking.Domain.DomainEvents;


namespace Booking.Domain.Interfaces.Services
{
    public interface IEmbaddingService
    {
        Task EmbaddeRoomCreatedEvent(RoomCreatedDomainEvent domainEvent, CancellationToken ct = default);
        Task EmbaddeListingCreatedEvent(LisitngCreatedDomainEvent domainEvent, CancellationToken ct = default);
    }
}
