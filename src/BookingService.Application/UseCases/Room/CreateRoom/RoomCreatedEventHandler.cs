using Booking.Domain.DomainEvents;
using MediatR;

namespace Booking.Application.UseCases.Room.CreateRoom
{
    public sealed class RoomCreatedEventHandler : INotificationHandler<RoomCreatedDomainEvent>
    {
        public Task Handle(RoomCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
