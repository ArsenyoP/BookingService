using Booking.Domain.Entities;
using Booking.Domain.Enums;
using FluentAssertions;

namespace BookingService.UnitTests.DomainTests
{
    public class DomainUserTests
    {
        private static User CreateTestUser()
        {
            var adultBirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20));
            return User.Create("John", "Doe", adultBirthDate, "john.doe@example.com", "johndoe").Value!;
        }

        [Fact]
        public void Create_ValidAdult_IsSuccessTrue()
        {
            var firstName = "John";
            var lastName = "Doe";
            var adultBirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));
            var email = "john.doe@example.com";
            var username = "johndoe";

            var result = User.Create(firstName, lastName, adultBirthDate, email, username);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.FirstName.Should().Be(firstName);
            result.Value.LastName.Should().Be(lastName);
            result.Value.DateOfBirth.Should().Be(adultBirthDate);
            result.Value.Email.Should().Be(email);
            result.Value.UserName.Should().Be(username);
            result.Value.Role.Should().Be(UserRole.Guest);
            result.Value.IsActive.Should().BeTrue();
            result.Value.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Create_UnderageUser_IsSuccessFalse()
        {
            var underageBirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-17));

            var result = User.Create("John", "Doe", underageBirthDate, "john.doe@example.com", "johndoe");

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("User.UnderAge");
            result.Value.Should().BeNull();
        }

        [Fact]
        public void IsAdult_ExactlyEighteenYearsOld_IsSuccessTrue()
        {
            var exactlyEighteen = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-18));

            var result = User.IsAdult(exactlyEighteen);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void SetRole_Admin_ChangesRoleToAdmin()
        {
            var user = CreateTestUser();

            var result = user.SetRole("Admin");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("Admin");
            user.Role.Should().Be(UserRole.Admin);
        }

        [Fact]
        public void SetRole_Host_ChangesRoleToHost()
        {
            var user = CreateTestUser();

            var result = user.SetRole("Host");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("Host");
            user.Role.Should().Be(UserRole.Host);
        }

        [Fact]
        public void SetRole_Guest_ChangesRoleToGuest()
        {
            var user = CreateTestUser();
            user.SetRole("Admin");

            var result = user.SetRole("Guest");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("Guest");
            user.Role.Should().Be(UserRole.Guest);
        }

        [Theory]
        [InlineData("Manager")]
        [InlineData("SuperAdmin")]
        [InlineData("")]
        [InlineData(null)]
        public void SetRole_InvalidRole_IsSuccessFalse(string invalidRole)
        {
            var user = CreateTestUser();

            var result = user.SetRole(invalidRole);

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("User.RoleNotExists");
            user.Role.Should().Be(UserRole.Guest);
        }
    }
}
