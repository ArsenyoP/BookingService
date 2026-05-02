using FluentValidation;

namespace Booking.Application.UseCases.Amenities.AddAmenityToListing
{
    public sealed class AddAmenityToListingValidator : AbstractValidator<AddAmenityToListingCommand>
    {
        public AddAmenityToListingValidator()
        {
            RuleFor(x => x.ListingId).NotEmpty().WithMessage("Listing id is required.");
            RuleFor(x => x.AmenityName).NotEmpty().WithMessage("Amenity name is required.");
        }
    }
}
