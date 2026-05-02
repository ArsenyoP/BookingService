using FluentValidation;

namespace Booking.Application.UseCases.Amenities.RemoveAmenityFromRoom
{
    public sealed class RemoveAmenityFromRoomValidator : AbstractValidator<RemoveAmenityFromRoomCommand>
    {
        public RemoveAmenityFromRoomValidator()
        {
            RuleFor(x => x.roomId).NotEmpty().WithMessage("Room id is required.");
            RuleFor(x => x.amenityName).NotEmpty().WithMessage("Amenity name is required.");
        }
    }
}
