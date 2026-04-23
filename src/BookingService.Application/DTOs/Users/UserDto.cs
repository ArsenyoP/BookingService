using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.DTOs.Users
{
    public sealed record UserDto(
        string UserName,
        string FirstName,
        string LastName,
        string Email,
        string JwtToken);
}
