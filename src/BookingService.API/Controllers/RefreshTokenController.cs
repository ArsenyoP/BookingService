using Booking.Application.UseCases.Users.InvalidateRefreshToken;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Booking.API.Controllers
{
    [ApiController]
    [EnableRateLimiting("auth-limiter")]
    [Route("api/refresoToken")]
    public class RefreshTokenController(ISender _sender) : ControllerBase
    {
        [HttpDelete("invalidate")]
        public async Task<IActionResult> Register([FromQuery] Guid userId, CancellationToken ct = default)
        {
            var command = new InvalidateRefreshTokenCommand(userId);
            var result = await _sender.Send(command, ct);
            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }
    }
}
