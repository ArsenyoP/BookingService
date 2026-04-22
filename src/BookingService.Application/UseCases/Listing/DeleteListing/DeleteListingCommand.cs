using Booking.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.UseCases.Listing.DeleteListing
{
    public sealed record DeleteListingCommand(Guid ListingId) : ICommand<Guid>;
}
