using Booking.Application.Abstractions;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Booking.Application.Behaviors
{
    public sealed class CachingBehavior<TRequest, TResponse>(ICacheService _cache,
        ILogger<CachingBehavior<TRequest, TResponse>> _logger)
    : IPipelineBehavior<TRequest, Result<TResponse>>
        where TRequest : IRequest<Result<TResponse>>, ICachableQuery
    {
        public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken cancellationToken)
        {
            if (request is not ICachableQuery cacheable)
                return await next();

            var cached = await _cache.GetAsync<TResponse>(cacheable.Key, cancellationToken);

            if (cached is not null)
            {
                _logger.LogInformation($"Returned from cache with key: {cacheable.Key}, data: {cached}");
                return Result<TResponse>.Success(cached);
            }

            var response = await next();

            if (response.Value is null)
                return response;

            try
            {
                await _cache.SetAsync<TResponse>(
                    cacheable.Key,
                    response.Value,
                    cacheable.Expiration,
                    cancellationToken);

                _logger.LogInformation("Successfully cached value with key: {Key}", request.Key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set cache for key: {Key}", request.Key);
            }

            return response;
        }
    }
}
