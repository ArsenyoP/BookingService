using Booking.Application.Abstractions;
using Booking.Application.Interfaces;
using Booking.Domain.Common;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;

namespace Booking.Application.UseCases.Booking.DeleteBooking
{
    public sealed class DeleteBookingHandler(
        IBookingRepository _bookingRepository,
        IBookingRepository _bokingRepository,
        IUnitOfWork _unitOfWork) : ICommandHandler<DeleteBookingCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(DeleteBookingCommand request, CancellationToken ct)
        {
            var booking = await _bokingRepository.GetById(request.BookingId, ct);

            if (booking is null)
            {
                return Result<Guid>.Failure(BookingErrors.NotFound);
            }

            _bookingRepository.Delete(booking);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<Guid>.Success(booking.Id);
        }
    }
}