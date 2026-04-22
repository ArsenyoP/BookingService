using Booking.Application.UseCases.Room.CreateRoom;
using Booking.Application.UseCases.Room.DeleteRoom;
using Booking.Application.UseCases.Room.GetAllRooms;
using Booking.Application.UseCases.Room.GetById;
using Booking.Application.UseCases.Room.GetByListingId;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Booking.API.Controllers
{
    [Route("api/room")]
    [EnableRateLimiting("fixed")]
    [ApiController]
    public class RoomControllers(ISender _sender) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetAllRoomsQuery(page, pageSize), ct);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetByIdQuery(id), ct);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpGet("listingId/{id:guid}")]
        public async Task<IActionResult> GetByListingId(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetByListingIdQuery(page, pageSize, id), ct);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [EnableRateLimiting("write-limiter")]
        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomCommand command, CancellationToken ct)
        {
            var result = await _sender.Send(command, ct);

            return result.IsSuccess
                ? Created($"/api/bookings/{result.Value}", result.Value)
                : BadRequest(result.Error);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteRoom([FromQuery] Guid id, CancellationToken ct)
        {

            var command = new DeleteRoomCommand(id);
            var result = await _sender.Send(command, ct);

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }
    }
}
