using Booking.Application.Abstractions;
using Booking.Application.Interfaces;
using Booking.Domain.Common;
using Booking.Domain.Entities;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Domain.Interfaces.Services;

namespace Booking.Application.UseCases.Users.LoginWithRefreshToken
{
    public sealed class LoginWithRefreshTokenHandler(IRefreshTokenRepository _refreshTokenRepository,
        ITokenService _tokenService, IUnitOfWork _unitOfWork) : ICommandHandler<LoginWithRefreshTokenCommand, LoginWithRefreshTokenResponse>
    {
        public async Task<Result<LoginWithRefreshTokenResponse>> Handle(LoginWithRefreshTokenCommand request, CancellationToken ct)
        {
            RefreshToken? refreshToken = await _refreshTokenRepository.GetById(request.RefreshToken);

            if (refreshToken is null || refreshToken.ExpiresOnUtc <= DateTime.UtcNow)
            {
                return Result<LoginWithRefreshTokenResponse>.Failure(new Error("RefreshToken.Expired", "Refresh token has expired"));
            }

            string accessToken = await _tokenService.CreateToken(refreshToken.User);

            string refreshTokenString = _tokenService.GenerateRefreshToken();

            refreshToken.Token = refreshTokenString;
            refreshToken.ExpiresOnUtc = DateTime.UtcNow.AddDays(7);

            await _unitOfWork.SaveChangesAsync(ct);

            return Result<LoginWithRefreshTokenResponse>.Success(new LoginWithRefreshTokenResponse(accessToken, refreshToken.Token));
        }
    }
}
