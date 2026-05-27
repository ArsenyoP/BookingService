using Booking.Application.UseCases.Users.LoginUser;
using Booking.Application.UseCases.Users.LoginWithRefreshToken;
using Booking.Application.UseCases.Users.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/auth")
                           .RequireRateLimiting("auth-limiter")
                           .WithTags("Auth");

            group.MapPost("/register", Register);
            group.MapPost("/login", Login);
            group.MapPost("/login/refreshToken", LoginWithRefreshToken);
        }


        private static async Task<IResult> Register(
            [FromBody] RegisterUserCommand command,
            ISender _sender,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(command, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> Login(
            [FromBody] LoginUserCommand command,
            ISender _sender,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(command, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> LoginWithRefreshToken(
            [FromBody] LoginWithRefreshTokenCommand command,
            ISender _sender,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(command, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }
    }
}