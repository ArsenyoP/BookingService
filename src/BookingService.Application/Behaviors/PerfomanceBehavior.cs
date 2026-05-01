using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Booking.Application.Behaviors
{
    internal class PerfomanceBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> _logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var timer = Stopwatch.StartNew();

            var response = await next();

            timer.Stop();

            var elapsed = timer.ElapsedMilliseconds;

            var requsetName = typeof(TRequest).Name;
            if(elapsed > 500)
            {
                _logger.LogWarning("low request {RequestName} took {Elapsed} ms",
                    requsetName, elapsed);
                return response;
            }

            _logger.LogInformation("Request {RequestName} took {Elapsed} ms",
                requsetName, elapsed);

            return response;
        }
    }
}
