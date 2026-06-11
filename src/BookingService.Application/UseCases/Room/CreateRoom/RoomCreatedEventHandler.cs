using Booking.Domain.DomainEvents;
using Booking.Domain.Interfaces.Services;
using MediatR;

namespace Booking.Application.UseCases.Room.CreateRoom
{
    public sealed class RoomCreatedEventHandler(IEmbaddingService _embaddingService) : INotificationHandler<RoomCreatedDomainEvent>
    {
        public async Task Handle(RoomCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            await _embaddingService.EmbaddeRoomCreatedEvent(domainEvent, cancellationToken);
        }
    }
}
