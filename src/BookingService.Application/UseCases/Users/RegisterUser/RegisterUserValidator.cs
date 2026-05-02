using FluentValidation;

namespace Booking.Application.UseCases.Users.RegisterUser
{
    public sealed class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.registerDto).NotNull().WithMessage("Registration data is required.");
            RuleFor(x => x.registerDto.UserName).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.registerDto.FirstName).NotEmpty().WithMessage("First name is required.");
            RuleFor(x => x.registerDto.LastName).NotEmpty().WithMessage("Last name is required.");
            RuleFor(x => x.registerDto.Email).NotEmpty().EmailAddress().WithMessage("Email is invalid.");
            RuleFor(x => x.registerDto.Password).NotEmpty().MinimumLength(8).WithMessage("Password must be at least 8 characters.");
            RuleFor(x => x.registerDto.DateOfBirth).LessThan(DateOnly.FromDateTime(DateTime.UtcNow.Date)).WithMessage("Date of birth must be in the past.");
            RuleFor(x => x.role).NotEmpty().WithMessage("Role is required.");
        }
    }
}
