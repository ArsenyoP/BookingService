using Booking.Application.Abstractions;
using Booking.Domain.Common;
using Booking.Domain.Interfaces.IRepositories;

namespace Booking.Application.UseCases.Users.InvalidateRefreshToken
{
    public class InvalidateRefreshTokenHandler(IRefreshTokenRepository _refreshRepo) : ICommandHandler<InvalidateRefreshTokenCommand, bool>
    {
        public async Task<Result<bool>> Handle(InvalidateRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            await _refreshRepo.InvalidateUsersToken(request.userId);
            return Result<bool>.Success(true);
        }
    }
}
