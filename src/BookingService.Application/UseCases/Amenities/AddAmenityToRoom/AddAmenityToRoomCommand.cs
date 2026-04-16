using Booking.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.UseCases.Amenities.AddAmenityToRoom
{
    public sealed record AddAmenityToRoomCommand(Guid RoomId, string AmenityName) : ICommand<Guid>;
}
