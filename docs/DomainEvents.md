# Domain Events in BookingService

## Overview

Domain events represent significant occurrences within the domain model that trigger side effects outside the aggregate boundary (e.g., sending emails, updating search indexes, triggering workflows). They enable loose coupling between domain entities and application services while maintaining consistency within a transaction.

## How Domain Events Work

### 1. Definition
Domain events are simple POCO classes that implement the `IDomainEvent` marker interface, which inherits from MediatR's `INotification`. This allows them to be published via MediatR's publisher/subscriber mechanism.

**Example:** `BookingCreatedDomainEvent.cs`
```csharp
using MediatR;

namespace Booking.Domain.DomainEvents
{
    public record BookingCreatedDomainEvent(Guid BookingId) : IDomainEvent;
}
```

### 2. Raising Events
Entities that inherit from `AggregateRoot` can raise domain events using the protected `RaiseDomainEvent` method. This adds the event to a private collection within the entity.

**Example:** In `Listing.Create`
```csharp
var listing = new Listing(title, description, address, listingType);
// ...
listing.RaiseDomainEvent(new LisitngCreatedDomainEvent(
    listing.Id, 
    listing.Address.City,
    listing.Address.Street, 
    searchText));
```

The `AggregateRoot` base class:
```csharp
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);

    public IReadOnlyCollection<IDomainEvent> GetDomainEvents()
        => _domainEvents.ToList();

    public void ClearDomainEvents()
        => _domainEvents.Clear();
}
```

### 3. Persisting Events (Outbox Pattern)
To guarantee event publication even if the process crashes after saving the entity but before sending the event, the system uses the **Outbox pattern**:

- An EF Core `SaveChangesInterceptor` (`ConvertDomainEventToOutboxMessageInterceptor`) intercepts `SavingChangesAsync`.
- It scans the change tracker for `AggregateRoot` instances, retrieves their pending domain events via `GetDomainEvents()`, clears them from the entities, and serializes each event into an `OutboxMessage` row.
- The `OutboxMessage` table stores:
  - `Id`: Unique identifier
  - `Type`: Event type name
  - `Content`: JSON-serialized event (with type name handling for deserialization)
  - `OccurredOnUtc`: Timestamp
  - `ProcessedOnUtc`: Null until processed
  - `Error`: Any error message if processing fails

**Interceptor code:** `ConvertDomainEventToOutboxMessageInterceptor.cs`

### 4. Publishing Events
A background job (`ProcessOutboxMessageJob`) runs periodically (every 10 seconds via Quartz) to:

1. Query unprocessed `OutboxMessage` rows (`ProcessedOnUtc == null`), ordered by occurrence.
2. Deserialize each message's `Content` back to an `IDomainEvent` instance.
3. Publish the event using MediatR's `IPublisher.Publish`.
4. On success, set `ProcessedOnUtc` to mark the message as processed.
5. On failure, log the error and store the exception message in the `Error` field (allowing retry).

**Job code:** `ProcessOutboxMessageJob.cs`

### 5. Handling Events
MediatR dispatches published events to all registered `INotificationHandler<TEvent>` handlers. These handlers reside in the application layer, typically within the feature's use case folders, and perform side effects such as:

- Sending confirmation emails (`BookingCreatedEventHandler`)
- Updating external search indexes via an embedding service (`CreateLisitngEventHandler`, `RoomCreatedEventHandler`)
- Any other asynchronous or external side effect

**Example handler:** `BookingCreatedEventHandler.cs`
```csharp
public async Task Handle(BookingCreatedDomainEvent notification, CancellationToken cancellationToken)
{
    var emailData = await bookingQueries.GetConfirmationEmailDataAsync(
        notification.BookingId, cancellationToken);
    // ... construct and send email ...
    await emailService.SendBookingConfirmationAsync(...);
}
```

## Flow Summary

1. **Domain Action** (e.g., creating a listing via `Listing.Create`) → Entity raises domain event via `RaiseDomainEvent`.
2. **Transaction Commit** (`SaveChanges`) → Interceptor moves events from entities to `OutboxMessage` table (still within same DB transaction).
3. **Background Job** (runs every 10s) → Reads unprocessed outbox messages, deserializes, publishes via MediatR.
4. **Event Handling** → MediatR notifies all handlers, which execute side effects (emails, external calls, etc.).
5. **Completion** → Job marks message as processed; failures are logged and retried on next interval.

## Files Involved

### Domain Layer
- `src/BookingService.Domain/Common/IDomainEvent.cs` – Marker interface for domain events.
- `src/BookingService.Domain/Common/AggregateRoot.cs` – Base class with event raising/collection logic.
- `src/BookingService.Domain/Entities/Listing.cs` – Example entity raising `LisitngCreatedDomainEvent`.
- `src/BookingService.Domain/Entities/Room.cs` – Example entity raising `RoomCreatedDomainEvent`.
- `src/BookingService.Domain/Entities/Bookings.cs` – Example entity raising `BookingCreatedDomainEvent`.
- `src/BookingService.Domain/DomainEvents/LisitngCreatedDomainEvent.cs` – Event for listing creation.
- `src/BookingService.Domain/DomainEvents/RoomCreatedDomainEvent.cs` – Event for room creation.
- `src/BookingService.Domain/DomainEvents/BookingCreatedDomainEvent.cs` – Event for booking creation.

### Infrastructure Layer
- `src/BookingService.Infrastructure/Interceptors/ConvertDomainEventToOutboxMessageInterceptor.cs` – EF Core interceptor that persists events to outbox.
- `src/BookingService.Infrastructure/Persistence/OutboxMessage.cs` – Outbox message entity.
- `src/BookingService.Infrastructure/Data/AppDbContext.cs` – DbContext with `DbSet<OutboxMessage>`.
- `src/BookingService.Infrastructure/BackgroundJobs/ProcessOutboxMessageJob.cs` – Quartz job that publishes outbox messages.
- `src/BookingService.Infrastructure/DependencyInjection.cs` – Registers interceptor, configures DbContext with interceptor, sets up Quartz job.

### Application Layer
- `src/BookingService.Application/DependencyInjection.cs` – Registers MediatR and pipelines.
- `src/BookingService.Application/UseCases/Listing/CreateListing/CreateLisitngEventHandler.cs` – Handles listing created event.
- `src/BookingService.Application/UseCases/Bookings/CreateBooking/BookingCreatedEventHandler.cs` – Handles booking created event.
- `src/BookingService.Application/UseCases/Room/CreateRoom/RoomCreatedEventHandler.cs` – Handles room created event.

### Notes
- There are minor typos in the codebase (`Lisitng` instead of `Listing`) in event and handler names; the functionality remains correct.
- The outbox pattern ensures at-least-once delivery; duplicates are handled by idempotent handlers if needed.
- The background job interval is configurable via Quartz (currently 10 seconds).