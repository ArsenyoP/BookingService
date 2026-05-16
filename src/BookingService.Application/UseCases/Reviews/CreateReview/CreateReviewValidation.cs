using FluentValidation;

namespace Booking.Application.UseCases.Reviews.CreateReview
{
    public sealed class CreateReviewValidation : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewValidation()
        {
            RuleFor(x => x.createReviewDto.TargetId)
                .NotEmpty().WithMessage("TargetId is required.");

            RuleFor(x => x.createReviewDto.TargetType)
                .IsInEnum().WithMessage("Invalid target type (must be Room or Listing).");

            RuleFor(x => x.createReviewDto.Score)
                .InclusiveBetween(1, 5).WithMessage("Score must be between 1 and 5.");

            RuleFor(x => x.createReviewDto.Text)
                .NotEmpty().WithMessage("Review text cannot be empty.")
                .MinimumLength(10).WithMessage("Review must be at least 10 characters long.")
                .MaximumLength(1000).WithMessage("Review cannot exceed 1000 characters.");
        }
    }
}
