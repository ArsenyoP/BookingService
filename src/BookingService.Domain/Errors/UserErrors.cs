using Booking.Domain.Common;

namespace Booking.Domain.Errors
{
    public static class UserErrors
    {
        public static readonly Error EmailAlreadyExists =
            new("User.EmailAlreadyExists", "User with this email already exists");

        public static readonly Error UnderAge =
            new("User.UnderAge", "User must be older than 18 y.o");

        public static readonly Error InvalidCredentials =
            new("User.InvalidCredentials", "Unvalide email or password");

        public static readonly Error NotFound =
            new("User.NotFound", "Can't find user");

        public static readonly Error AlreadyInactive =
            new("User.AlreadyInactive", "Account already is deactivated");

        public static readonly Error AccountInactive =
            new("User.AccountInactive", "Account is deactivated");
    }
}
