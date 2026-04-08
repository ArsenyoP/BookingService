using Booking.Application.UseCases.Room.CreateRoom;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Route("api/room")]
    [ApiController]
    public class RoomControllers(ISender _sender) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomCommand command, CancellationToken ct)
        {
            var result = await _sender.Send(command, ct);

            return result.IsSuccess
                ? Created($"/api/bookings/{result.Value}", result.Value)
                : BadRequest(result.Error);
        }
    }
}
