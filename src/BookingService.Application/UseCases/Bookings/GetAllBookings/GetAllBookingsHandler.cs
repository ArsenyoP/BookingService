using Booking.Application.Abstractions;
using Booking.Application.DTOs.Bookings;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Common;


namespace Booking.Application.UseCases.Bookings.GetAllBookings
{
    internal class GetAllBookingsHandler(IBookingQueries _bookingQueries) : IQueryHandler<GetAllBookingsQuery, IReadOnlyList<BookingResponseDto>>
    {

        public async Task<Result<IReadOnlyList<BookingResponseDto>>> Handle(GetAllBookingsQuery request, CancellationToken ct)
        {
            //TODO: Add validation throught FluentValidation
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var bookings = await _bookingQueries.GetAllPagedAsync(page, pageSize, ct);

            return Result<IReadOnlyList<BookingResponseDto>>.Success(bookings);
        }
    }
}
