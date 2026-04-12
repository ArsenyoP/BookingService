using MediatR;
using Microsoft.AspNetCore.Mvc;
using Booking.Application.UseCases.Bookings.CreateBooking;
using Booking.Application.UseCases.Bookings.GetAllBookings;
using Booking.Application.UseCases.Bookings.GetById;

namespace Booking.API.Controllers
{

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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingCommand command, CancellationToken ct)
        {
            var result = await _sender.Send(command, ct);
            return result.IsSuccess
                ? Created($"/api/bookings/{result.Value}", result.Value)
                : BadRequest(result.Error);
        }

    }
}
