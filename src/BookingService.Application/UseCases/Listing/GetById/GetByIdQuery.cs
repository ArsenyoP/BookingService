using Booking.Application.Abstractions;
using Booking.Application.DTOs.Listings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.UseCases.Listing.GetById
{
    public sealed record GetByIdQuery(Guid id) : IQuery<ListingResponseDto>;

}
