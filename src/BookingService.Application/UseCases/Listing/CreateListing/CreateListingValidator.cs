using FluentValidation;

namespace Booking.Application.UseCases.Listing.CreateListing
{
    public sealed class CreateListingValidator : AbstractValidator<CreateListingCommand>
    {
        public CreateListingValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
            RuleFor(x => x.Country).NotEmpty().WithMessage("Country is required.");
            RuleFor(x => x.City).NotEmpty().WithMessage("City is required.");
            RuleFor(x => x.Street).NotEmpty().WithMessage("Street is required.");
            RuleFor(x => x.HouseNumber).NotEmpty().WithMessage("House number is required.");
            RuleFor(x => x.Floor).GreaterThanOrEqualTo(0).WithMessage("Floor cannot be negative.");
            RuleFor(x => x.ListingType).IsInEnum().WithMessage("Listing type is invalid.");
        }
    }
}
