using Booking.Application.Abstractions;
using Booking.Application.DTOs.Amenities;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.UseCases.Amenities.GetAllAmenities
{
    public class GetAllAmenitiesHandler(IAmenityQueries _amenityQueries) : IQueryHandler<GetAllAmenitiesQuery, IReadOnlyList<AmenityDto>>
    {
        public async Task<Result<IReadOnlyList<AmenityDto>>> Handle(GetAllAmenitiesQuery request, CancellationToken ct)
        {
            var amenities = await _amenityQueries.GetAllAmenities(request.Page, request.PageSize, ct);

            return Result<IReadOnlyList<AmenityDto>>.Success(amenities);
        }
    }
}
