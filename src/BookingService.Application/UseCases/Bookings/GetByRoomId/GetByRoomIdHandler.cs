using Booking.Application.Abstractions;
using Booking.Application.DTOs.Bookings;
using Booking.Application.Queries;
using Booking.Domain.Common;
using Booking.Domain.Errors;
using Booking.Application.Interfaces.IQueries;

namespace Booking.Application.UseCases.Bookings.GetByRoomId
{
    public sealed class GetByRoomIdHandler(IBookingQueries _bookingQueries, IRoomQueries _roomQueries) : IQueryHandler<GetByRoomIdQuery, IReadOnlyList<BookingResponseDto>>
    {
        public async Task<Result<IReadOnlyList<BookingResponseDto>>> Handle(GetByRoomIdQuery request, CancellationToken ct)
        {
            var isExists = await _roomQueries.GetByIdAsync(request.roomId) is not null;

            if (!isExists)
            {
                return Result<IReadOnlyList<BookingResponseDto>>.Failure(RoomErrors.NotFound);
            }

            var result = await _bookingQueries.GetByRoomPagedAsync(request.roomId, request.page, request.pageSize, ct);

            if (result is null)
            {
                return Result<IReadOnlyList<BookingResponseDto>>.Failure(BookingErrors.NotFound);
            }

            return Result<IReadOnlyList<BookingResponseDto>>.Success(result);
        }
    }
}
