using Booking.Domain.Common;
using Booking.Domain.Enums;
using Booking.Domain.Errors;
using Microsoft.AspNetCore.Identity;


namespace Booking.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole Role { get; set; } = UserRole.Guest;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public DateOnly DateOfBirth { get; set; }
        public bool IsActive { get; set; } = true;


        private User() { }

        private User(string firstName, string lastName, DateOnly dateOfBirth, string email, string username)
        {
            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Email = email;
            UserName = username;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public static Result IsAdult(DateOnly dateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (dateOfBirth.AddYears(18) > today)
            {
                return Result.Failure(UserErrors.UnderAge);
            }
            return Result.Success();
        }

        public static Result<User> Create(string firstName, string lastName, DateOnly dateOfBirth, string email, string username)
        {
            var ageResult = IsAdult(dateOfBirth);
            if (!ageResult.IsSuccess)
            {
                return Result<User>.Failure(ageResult.Error);
            }

            var user = new User(firstName, lastName, dateOfBirth, email, username);

            return Result<User>.Success(user);
        }
    }
}
