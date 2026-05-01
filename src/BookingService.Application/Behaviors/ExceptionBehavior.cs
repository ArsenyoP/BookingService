using MediatR;
using Microsoft.Extensions.Logging;

namespace Booking.Application.Behaviors
{
    public class ExceptionBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName  = typeof(TRequest).Name;

            //TODO: Different responses for different exceptions
            try
            {
                return await next();
            }
            catch(Exception ex) 
            {
                throw;
            }
        }
    }
}
