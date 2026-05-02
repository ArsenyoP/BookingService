using FluentValidation;

namespace Booking.Application.UseCases.Bookings.CreateBooking
{
    public sealed class CreateBookingValidator : AbstractValidator<CreateBookingCommand>
    {
        public CreateBookingValidator()
        {
            RuleFor(x => x.RoomId).NotEmpty().WithMessage("Room id is required.");
            RuleFor(x => x.GuestId).NotEmpty().WithMessage("Guest id is required.");
            RuleFor(x => x.StartDate).LessThan(x => x.EndDate).WithMessage("Start date must be before end date.");
            RuleFor(x => x.AdultsCount).GreaterThan(0).WithMessage("Adults count must be greater than 0.");
            RuleFor(x => x.ChildrenCount).GreaterThanOrEqualTo(0).WithMessage("Children count cannot be negative.");
        }
    }
}
