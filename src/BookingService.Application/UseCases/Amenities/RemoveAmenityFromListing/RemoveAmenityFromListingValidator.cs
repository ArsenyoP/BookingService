using FluentValidation;

namespace Booking.Application.UseCases.Amenities.RemoveAmenityFromListing
{
    public sealed class RemoveAmenityFromListingValidator : AbstractValidator<RemoveAmenityFromListingCommand>
    {
        public RemoveAmenityFromListingValidator()
        {
            RuleFor(x => x.listingId).NotEmpty().WithMessage("Listing id is required.");
            RuleFor(x => x.amenityName).NotEmpty().WithMessage("Amenity name is required.");
        }
    }
}
