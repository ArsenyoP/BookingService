using FluentValidation;

namespace Booking.Application.UseCases.Room.CreateRoom;

public sealed class CreateRoomValidator : AbstractValidator<CreateRoomCommand>
{
    public CreateRoomValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
        RuleFor(x => x.Type).IsInEnum().WithMessage("Room type is invalid.");
        RuleFor(x => x.PricePerNight).GreaterThan(0).WithMessage("Price per night must be greater than 0.");
        RuleFor(x => x.AdultsCapacity).GreaterThan(0).WithMessage("Adults capacity must be greater than 0.");
        RuleFor(x => x.ChildrenCapacity).GreaterThanOrEqualTo(0).WithMessage("Children capacity cannot be negative.");
        RuleFor(x => x.ListingId).NotEmpty().WithMessage("Listing id is required.");
    }
}
