using Booking.Application.Abstractions;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Common;
using MediatR;

namespace Booking.Application.Behaviors
{
    public sealed class CachingBehavior<TRequest, TResponse>(ICacheService _cache)
    : IPipelineBehavior<TRequest, Result<TResponse>>
        where TRequest : IRequest<Result<TResponse>>, ICachableQuery
    {
        public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken cancellationToken)
        {
            if (request is not ICachableQuery cacheable)
                return await next();

            var cached = await _cache.GetAsync<TResponse>(cacheable.Key, cancellationToken);

            //if data in cache
            if (cached is not null)
                return Result<TResponse>.Success(cached);

            var response = await next();

            if (response.Value is null)
                return response;

            await _cache.SetAsync<TResponse>(cacheable.Key,
                response.Value, cacheable.Expiration, cancellationToken);


            return response;
        }
    }
}
