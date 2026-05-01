using MediatR;
using Microsoft.Extensions.Logging;

namespace Booking.Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> _logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogInformation("Start: {requestName}", requestName);

            try
            {
                var response = await next();

                _logger.LogInformation("Succeded request: {requestName}", requestName);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {requestName}", requestName);
                throw;
            }
            finally
            {
                _logger.LogInformation("Request {requestName} finished", requestName);
            }


        }
    }
}
