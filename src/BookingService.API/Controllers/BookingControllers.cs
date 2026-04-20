using Booking.Application.UseCases.Bookings.CreateBooking;
using Booking.Application.UseCases.Bookings.GetAllBookings;
using Booking.Application.UseCases.Bookings.GetById;
using Booking.Application.UseCases.Bookings.GetByRoomId;
using Booking.Application.UseCases.Bookings.GetByUserId;
using Booking.Application.UseCases.Bookings.IsRoomAvailable;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Booking.API.Controllers
{
    [EnableRateLimiting("fixed")]
    [ApiController]
    [Route("api/bookings")]
    public class BookingControllers(ISender _sender) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetAllBookingsQuery(page, pageSize), ct);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetByIdQuery(id), ct);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpGet("room/{id:guid}")]
        public async Task<IActionResult> GetByRoomId(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetByRoomIdQuery(id, page, pageSize), ct);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpGet("user/{id:guid}")]
        public async Task<IActionResult> GetByUserId(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetByUserIdQuery(id, page, pageSize), ct);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpGet("roomBool/{id:guid}")]
        public async Task<IActionResult> IsRoomAvailable(Guid id, [FromQuery] DateOnly start, [FromQuery] DateOnly end, CancellationToken ct = default)
        {
            var result = await _sender.Send(new IsRoomAvailableQuery(id, start, end), ct);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPost]
        [EnableRateLimiting("write-limiter")]
        public async Task<IActionResult> Create([FromBody] CreateBookingCommand command, CancellationToken ct)
        {
            var result = await _sender.Send(command, ct);
            return result.IsSuccess
                ? Created($"/api/bookings/{result.Value}", result.Value)
                : BadRequest(result.Error);
        }

    }
}
