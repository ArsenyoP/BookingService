using Booking.Domain.Common;
using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;

namespace Booking.Infrastructure.Interceptors
{
    public sealed class ConvertDomainEventToOutboxMessageInterceptor
        : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var _dbContext = eventData.Context;

            if (_dbContext is null)
            {
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            var events = _dbContext.ChangeTracker
                .Entries<AggregateRoot>()
                .Select(x => x.Entity)
                .SelectMany(aggregateRoot =>
                {
                    var domainEvents = aggregateRoot.GetDomainEvents();
                    aggregateRoot.ClearDomainEvents();

                    return domainEvents;
                })
                .Select(domainEvent => new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    OccurredOnUtc = DateTime.UtcNow,
                    Type = domainEvent.GetType().Name,
                    Content = JsonConvert.SerializeObject(domainEvent,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    })
                })
                .ToList();

            _dbContext.Set<OutboxMessage>().AddRange(events);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
