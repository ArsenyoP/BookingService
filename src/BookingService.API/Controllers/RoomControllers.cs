using Booking.Application.UseCases.Room.CreateRoom;
using Booking.Application.UseCases.Room.GetAllRooms;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Route("api/room")]
    [ApiController]
    public class RoomControllers(ISender _sender) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetAllRoomsQuery(page, pageSize), ct);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

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
