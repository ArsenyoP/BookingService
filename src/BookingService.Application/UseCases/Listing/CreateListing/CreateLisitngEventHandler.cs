using Booking.Domain.DomainEvents;
using Booking.Domain.Interfaces.Services;
using MediatR;

namespace Booking.Application.UseCases.Listing.CreateListing
{
    internal class CreateLisitngEventHandler(IEmbaddingService _embaddingService) : INotificationHandler<LisitngCreatedDomainEvent>
    {
        public async Task Handle(LisitngCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            await _embaddingService.EmbaddeListingCreatedEvent(notification, cancellationToken);
        }
    }
}
