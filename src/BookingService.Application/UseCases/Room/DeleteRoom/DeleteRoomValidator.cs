using FluentValidation;

namespace Booking.Application.UseCases.Room.DeleteRoom
{
    public sealed class DeleteRoomValidator : AbstractValidator<DeleteRoomCommand>
    {
        public DeleteRoomValidator()
        {
            RuleFor(x => x.roomId).NotEmpty().WithMessage("Room id is required.");
        }
    }
}
