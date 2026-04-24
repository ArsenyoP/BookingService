using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.DTOs.Users
{
    public sealed record RegisterDto(string UserName,
        string FirstName,
        string LastName,
        string Email,
        string Password,
        DateOnly DateOfBirth);
}
