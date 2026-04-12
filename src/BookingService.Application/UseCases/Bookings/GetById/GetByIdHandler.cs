using Booking.Application.Abstractions;
using Booking.Application.DTOs.Bookings;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Common;
using Booking.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.UseCases.Bookings.GetById
{
    public sealed class GetByIdHandler(IBookingQueries _bookingQueries) : IQueryHandler<GetByIdQuery, BookingResponseDto>
    {
        public async Task<Result<BookingResponseDto>> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _bookingQueries.GetByIdAsync(request.id);

            if (result is null)
            {
                return Result<BookingResponseDto>.Failure(BookingErrors.NotFound);
            }

            return Result<BookingResponseDto>.Success(result);
        }
    }
}
