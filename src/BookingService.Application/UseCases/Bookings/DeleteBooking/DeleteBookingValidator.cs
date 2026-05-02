using FluentValidation;

namespace Booking.Application.UseCases.Booking.DeleteBooking
{
    public sealed class DeleteBookingValidator : AbstractValidator<DeleteBookingCommand>
    {
        public DeleteBookingValidator()
        {
            RuleFor(x => x.BookingId).NotEmpty().WithMessage("Booking id is required.");
        }
    }
}
