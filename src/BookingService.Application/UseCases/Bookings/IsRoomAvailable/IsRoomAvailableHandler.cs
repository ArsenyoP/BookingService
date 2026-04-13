using Booking.Application.Abstractions;
using Booking.Application.Queries;
using Booking.Domain.Common;
using Booking.Domain.Errors;
using Booking.Application.Interfaces.IQueries;

namespace Booking.Application.UseCases.Bookings.IsRoomAvailable
{
    public sealed class IsRoomAvailableHandler(IBookingQueries _bookingQueries, IRoomQueries _roomQueries) : IQueryHandler<IsRoomAvailableQuery, bool>
    {
        public async Task<Result<bool>> Handle(IsRoomAvailableQuery request, CancellationToken cancellationToken)
        {
            var room = await _roomQueries.GetByIdAsync(request.roomId);

            if (room is null)
            {
                return Result<bool>.Failure(RoomErrors.NotFound);
            }

            if (request.start >= request.end)
            {
                return Result<bool>.Failure(DateRangeErrors.InvalidDate);
            }

            var result = await _bookingQueries.IsRoomAvailableAsync(request.roomId, request.start, request.end);

            return Result<bool>.Success(result);
        }
    }
}
