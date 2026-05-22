using Booking.Application.DTOs.Users;
using Booking.Application.Interfaces;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Common;
using Booking.Domain.Entities;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace Booking.Infrastructure.Services
{
    public class AuthService(UserManager<User> _userManager, SignInManager<User> _signInManager,
        ITokenService _tokenService, IUnitOfWork _unitOfWork, IRefreshTokenRepository _refreshRepo) : IAuthService
    {
        public async Task<Result<AuthResult>> LoginUser(LoginDto loginDto, CancellationToken ct = default)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user is null)
            {
                return Result<AuthResult>.Failure(UserErrors.NotFound);
            }

            var loginedUser = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!loginedUser.Succeeded)
            {
                return Result<AuthResult>.Failure(UserErrors.WrongAuthData);
            }

            var token = await _tokenService.CreateToken(user);



            var refreshTokenEntity = new RefreshToken(
                Guid.NewGuid(),
                _tokenService.GenerateRefreshToken(),
                user.Id,
                DateTime.UtcNow.AddDays(7)
                );

            _refreshRepo.Add(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync(ct);

            var userDto = new UserDto(
                user.UserName!,
                user.FirstName,
                user.LastName,
                user.Email!,
                token,
                refreshTokenEntity.Token);



            return Result<AuthResult>.Success(new AuthResult(userDto, refreshTokenEntity.Token));
        }

        public async Task<Result<AuthResult>> RegisterUser(RegisterDto registerDto, string role = "Guest", CancellationToken ct = default)
        {
            var allowedRoles = new[] { "Admin", "Guest", "Host" };
            if (!allowedRoles.Contains(role))
            {
                return Result<AuthResult>.Failure(UserErrors.RoleNotExists);
            }

            var userResult = User.Create(
                registerDto.FirstName,
                registerDto.LastName,
                registerDto.DateOfBirth,
                registerDto.Email,
                registerDto.UserName
            );

            if (!userResult.IsSuccess)
            {
                return Result<AuthResult>.Failure(userResult.Error);
            }

            var createdUserResult = await _userManager.CreateAsync(userResult.Value, registerDto.Password);

            if (createdUserResult.Succeeded)
            {
                var rolesResult = await _userManager.AddToRoleAsync(userResult.Value, role);
                if (rolesResult.Succeeded)
                {
                    var token = await _tokenService.CreateToken(userResult.Value);

                    var refreshTokenEntity = new RefreshToken(
                        Guid.NewGuid(),
                        _tokenService.GenerateRefreshToken(),
                        userResult.Value.Id,
                        DateTime.UtcNow.AddDays(7)
                        );

                    _refreshRepo.Add(refreshTokenEntity);
                    await _unitOfWork.SaveChangesAsync(ct);

                    userResult.Value.SetRole(role);

                    var registeredDto = new UserDto(
                        userResult.Value.UserName!,
                        userResult.Value.FirstName,
                        userResult.Value.LastName,
                        userResult.Value.Email!,
                        token,
                        refreshTokenEntity.Token);

                    return Result<AuthResult>.Success(new AuthResult(registeredDto, refreshTokenEntity.Token));
                }
                else
                {
                    await _userManager.DeleteAsync(userResult.Value);
                    var combinedError = string.Join(", ", rolesResult.Errors.Select(e => e.Description));
                    return Result<AuthResult>.Failure(new Error("Identity.MultipleErrors", combinedError));
                }
            }
            else
            {
                var combinedError = string.Join(", ", createdUserResult.Errors.Select(e => e.Description));
                return Result<AuthResult>.Failure(new Error("Identity.MultipleErrors", combinedError));
            }
        }


    }
}
