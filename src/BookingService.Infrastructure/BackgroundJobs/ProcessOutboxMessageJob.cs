using Booking.Domain.Common;
using Booking.Infrastructure.Data;
using Booking.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;

namespace Booking.Infrastructure.BackgroundJobs
{
    public class ProcessOutboxMessageJob(AppDbContext _dbContext,
        IPublisher _publisher, ILogger<ProcessOutboxMessageJob> _logger) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var messages = await _dbContext.Set<OutboxMessage>()
                .Where(x => x.ProcessedOnUtc == null)
                .Take(20)
                .OrderBy(x => x.OccurredOnUtc)
                .ToListAsync(context.CancellationToken);

            foreach (var message in messages)
            {
                try
                {
                    var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(message.Content, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    });

                    if (domainEvent is null)
                    {
                        _logger.LogWarning("Domain event in {message} is null", message);
                        continue;
                    }

                    await _publisher.Publish(domainEvent, context.CancellationToken);
                    message.ProcessedOnUtc = DateTime.UtcNow;
                    message.Error = null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process outbox message {Id}", message.Id);

                    message.Error = ex.Message;

                }
            }
            await _dbContext.SaveChangesAsync(context.CancellationToken);
        }
    }
}
