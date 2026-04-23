using Booking.Domain.Entities;
using Booking.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        public Task<string> CreateToken(User user)
        {
            throw new NotImplementedException();
        }
    }
}
