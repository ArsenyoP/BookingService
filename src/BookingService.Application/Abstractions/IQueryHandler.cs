using Booking.Domain.Common;
using MediatR;


namespace Booking.Application.Abstractions
{
    public interface IQueryHandler<TQuery, TResponse> :
        IRequestHandler<TQuery, Result<TResponse>>
        where TQuery : IQuery<TResponse>;
}
