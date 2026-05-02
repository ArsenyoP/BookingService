using FluentValidation;

namespace Booking.Application.UseCases.Amenities.DeleteAmenity
{
    public sealed class DeleteAmenityValidator : AbstractValidator<DeleteAmenityCommand>
    {
        public DeleteAmenityValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Amenity name is required.");
        }
    }
}
