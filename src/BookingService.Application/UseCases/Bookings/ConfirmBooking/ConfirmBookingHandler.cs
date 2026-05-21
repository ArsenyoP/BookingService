using Booking.Application.Abstractions;
using Booking.Application.Interfaces;
using Booking.Domain.Common;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;

namespace Booking.Application.UseCases.Bookings.ConfirmBooking
{
    public sealed class ConfirmBookingHandler(IBookingRepository _bookingRepository, IUnitOfWork _unitOfWork) : ICommandHandler<ConfirmBookingCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(ConfirmBookingCommand request, CancellationToken ct)
        {
            var booking = await _bookingRepository.GetBookingEntityByConfirmationToken(request.confirmationToken, ct);

            if (booking is null) return Result<Guid>.Failure(BookingErrors.NotFound);

            booking.Confirm();
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Success(booking.Id);
        }
    }
}
