using Booking.Domain.Common;
using MediatR;

namespace Booking.Application.Abstractions
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }
}
