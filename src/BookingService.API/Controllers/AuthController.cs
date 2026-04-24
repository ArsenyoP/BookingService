using Booking.Application.UseCases.Users.LoginUser;
using Booking.Application.UseCases.Users.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Booking.API.Controllers
{
    [ApiController]
    [EnableRateLimiting("auth-limiter")]
    [Route("api/auth")]
    public class AuthController(ISender _sender) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken ct = default)
        {
            var result = await _sender.Send(command, ct);
            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command, CancellationToken ct = default)
        {
            var result = await _sender.Send(command, ct);
            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }
    }
}
