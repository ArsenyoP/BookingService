using FluentValidation;

namespace Booking.Application.UseCases.Amenities.AddAmenityToRoom
{
    public sealed class AddAmenityToRoomValidator : AbstractValidator<AddAmenityToRoomCommand>
    {
        public AddAmenityToRoomValidator()
        {
            RuleFor(x => x.RoomId).NotEmpty().WithMessage("Room id is required.");
            RuleFor(x => x.AmenityName).NotEmpty().WithMessage("Amenity name is required.");
        }
    }
}
