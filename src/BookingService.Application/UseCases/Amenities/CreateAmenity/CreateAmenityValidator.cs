using FluentValidation;

namespace Booking.Application.UseCases.Amenities
{
    public sealed class CreateAmenityValidator : AbstractValidator<CreateAmenityCommand>
    {
        public CreateAmenityValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Amenity name is required.");
            RuleFor(x => x.Category).IsInEnum().WithMessage("Amenity category is invalid.");
        }
    }
}
