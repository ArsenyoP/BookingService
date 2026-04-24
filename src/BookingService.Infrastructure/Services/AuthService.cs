using Booking.Application.DTOs.Users;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Booking.Domain.Entities;
using Booking.Domain.Errors;
using Booking.Domain.Common;
using Booking.Domain.Interfaces.Services;

namespace Booking.Infrastructure.Services
{
    public class AuthService(UserManager<User> _userManager, ITokenService _tokenService) : IAuthService
    {
        public async Task<Result<UserDto>> RegisterUser(RegisterDto registerDto, string role = "Guest", CancellationToken ct = default)
        {
            var allowedRoles = new[] { "Admin", "Guest", "Host" };
            if (!allowedRoles.Contains(role))
            {
                return Result<UserDto>.Failure(UserErrors.RoleNotExists);
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
                return Result<UserDto>.Failure(userResult.Error);
            }

            var createdUserResult = await _userManager.CreateAsync(userResult.Value, registerDto.Password);

            if (createdUserResult.Succeeded)
            {
                var rolesResult = await _userManager.AddToRoleAsync(userResult.Value, role);
                if (rolesResult.Succeeded)
                {
                    var token = await _tokenService.CreateToken(userResult.Value);

                    userResult.Value.SetRole(role);

                    var registeredDto = new UserDto(
                        userResult.Value.UserName!,
                        userResult.Value.FirstName,
                        userResult.Value.LastName,
                        userResult.Value.Email!,
                        token);

                    return Result<UserDto>.Success(registeredDto);
                }
                else
                {
                    await _userManager.DeleteAsync(userResult.Value);
                    var combinedError = string.Join(", ", rolesResult.Errors.Select(e => e.Description));
                    return Result<UserDto>.Failure(new Error("Identity.MultipleErrors", combinedError));
                }
            }
            else
            {
                var combinedError = string.Join(", ", createdUserResult.Errors.Select(e => e.Description));
                return Result<UserDto>.Failure(new Error("Identity.MultipleErrors", combinedError));
            }
        }
    }
}
