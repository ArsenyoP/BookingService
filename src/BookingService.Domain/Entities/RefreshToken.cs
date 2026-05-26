using Booking.Domain.Common;

namespace Booking.Domain.Entities
{
    public class RefreshToken : Entity
    {
        public string Token { get; set; }
        public Guid UserId { get; private set; }
        public DateTime ExpiresOnUtc { get; set; }

        public User User { get; set; }

        public RefreshToken(
            Guid id,
            string token,
            Guid userId,
            DateTime expiresOnUtc)
        {
            Id = id;
            Token = token;
            UserId = userId;
            ExpiresOnUtc = expiresOnUtc;
        }
    }
}
