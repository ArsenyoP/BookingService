using FluentValidation;

namespace Booking.Application.UseCases.Listing.DeleteListing
{
    public sealed class DeleteListingValidator : AbstractValidator<DeleteListingCommand>
    {
        public DeleteListingValidator()
        {
            RuleFor(x => x.ListingId).NotEmpty().WithMessage("Listing id is required.");
        }
    }
}
