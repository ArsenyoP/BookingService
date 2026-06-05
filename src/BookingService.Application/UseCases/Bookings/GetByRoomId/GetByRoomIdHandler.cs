using Booking.Application.Abstractions;
using Booking.Application.DTOs.Bookings;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Common;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;

namespace Booking.Application.UseCases.Bookings.GetByRoomId
{
    public sealed class GetByRoomIdHandler(IRoomRepository _roomRepository, IBookingQueries _bookingQueries)
        : IQueryHandler<GetByRoomIdQuery, IReadOnlyList<BookingResponseDto>>
    {
        public async Task<Result<IReadOnlyList<BookingResponseDto>>> Handle(GetByRoomIdQuery request, CancellationToken ct)
        {
            var isExists = await _roomRepository.GetByIdWithAmenities(request.roomId) is not null;

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
