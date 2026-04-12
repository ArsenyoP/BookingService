using Booking.Application.Abstractions;
using Booking.Application.DTOs.Bookings;
using Booking.Application.Interfaces.IQueries;
using Booking.Application.Queries;
using Booking.Domain.Common;
using Booking.Domain.Entities;
using Booking.Domain.Errors;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.UseCases.Bookings.GetByUserId
{
    public sealed class GetByUserIdHandler(IBookingQueries _bookingQueries, UserManager<User> _userManager) : IQueryHandler<GetByUserIdQuery, IReadOnlyList<BookingResponseDto>>
    {
        public async Task<Result<IReadOnlyList<BookingResponseDto>>> Handle(GetByUserIdQuery request, CancellationToken ct)
        {
            var isExists = await _userManager.Users
                .AnyAsync(u => u.Id == request.userId, ct);

            if (!isExists)
            {
                return Result<IReadOnlyList<BookingResponseDto>>.Failure(UserErrors.NotFound);
            }

            var result = await _bookingQueries.GetByUserPagedAsync(request.userId, request.page, request.pageSize, ct);

            if (result is null)
            {
                return Result<IReadOnlyList<BookingResponseDto>>.Failure(BookingErrors.NotFound);
            }

            return Result<IReadOnlyList<BookingResponseDto>>.Success(result);
        }
    }
}
