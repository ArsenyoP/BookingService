using System.Security.Claims;

namespace Booking.Infrastructure.ExtensionMethods
{
    public static class ClaimsExtension
    {
        public static string GetUserID(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? user.FindFirstValue("sub")!;
        }
    }
}
