using Booking.Application.Abstractions;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Booking.Application.Behaviors
{
    public sealed class CacheInvalidationBehavior<TRequest, TResponse>(ICacheService _cache, ILogger<CacheInvalidationBehavior<TRequest, TResponse>> _logger)
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICacheInvalidationCommand
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next();
            _logger.LogInformation("Debugger hitted");

            if (response is IResult result && result.IsSuccess)
            {
                await _cache.RemoveAsync(request.Key, cancellationToken);
                _logger.LogInformation("Cache invalidated for key: {Key}", request.Key);
            }

            return response;
        }
    }
}
