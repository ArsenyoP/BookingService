using Booking.Domain.Common;
using MediatR;

namespace Booking.Application.Abstractions
{
    public interface ICommand : IRequest<Result> { }

    public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }
}