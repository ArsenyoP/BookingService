using MediatR;
using Microsoft.AspNetCore.Mvc;
using Booking.Application.UseCases.Bookings.CreateBooking;
using Booking.Application.UseCases.Bookings.GetAllBookings;

namespace Booking.API.Controllers
{

    [ApiController]
    [Route("api/bookings")]
    public class BookingControllers(ISender _sender) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _sender.Send(new GetAllBookingsQuery(), ct);

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
