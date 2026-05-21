using Booking.Application.Abstractions;
using Booking.Application.Interfaces;
using Booking.Domain.Common;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces;
using Booking.Domain.Interfaces.IRepositories;

namespace Booking.Application.UseCases.Bookings.CancelBooking
{
    public class CancelBookingHandler(IBookingRepository _bookingRepository,
        IUnitOfWork _unitOfWork, IRefundPolicy _refundPolicy) : ICommandHandler<CancelBookingCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(CancelBookingCommand request, CancellationToken ct)
        {
            var booking = await _bookingRepository.GetById(request.bookingId, ct);

            if (booking is null) return Result<Guid>.Failure(BookingErrors.NotFound);

            if (booking.GuestId != Guid.Parse(request.UserId)) return Result<Guid>.Failure(BookingErrors.CannotCancel);


            var today = DateTime.UtcNow;
            var cancellationResult = booking.Cancel(today, _refundPolicy);

            await _unitOfWork.SaveChangesAsync(ct);
            return Result<Guid>.Success(booking.Id);
        }
    }
}
