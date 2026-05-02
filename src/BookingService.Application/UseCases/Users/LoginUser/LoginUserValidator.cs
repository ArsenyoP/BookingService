using FluentValidation;

namespace Booking.Application.UseCases.Users.LoginUser
{
    public sealed class LoginUserValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserValidator()
        {
            RuleFor(x => x.LoginDto).NotNull().WithMessage("Login data is required.");
            RuleFor(x => x.LoginDto.UserName).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.LoginDto.Password).NotEmpty().WithMessage("Password is required.");
        }
    }
}
