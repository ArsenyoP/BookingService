using Booking.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Abstractions
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }
}
