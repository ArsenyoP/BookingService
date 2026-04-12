using Booking.Application.Abstractions;
using Booking.Application.Interfaces;
using Booking.Application.Queries;
using Booking.Domain.Common;
using Booking.Domain.Entities;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace Booking.Application.UseCases.Bookings.CreateBooking
{
    internal sealed class CreateBookingHandler(
        IBookingRepository _bookingRepository,
        IRoomQueries _roomQueries,
        UserManager<User> _userManager,
        IUnitOfWork unitOfWork) : ICommandHandler<CreateBookingCommand, Guid>
    {
        //TODO: fix overlapping
        public async Task<Result<Guid>> Handle(CreateBookingCommand request, CancellationToken ct)
        {
            var dateRangeResult = DateRange.Create(request.StartDate, request.EndDate);
            if (!dateRangeResult.IsSuccess || dateRangeResult.Value is null)
            {
                return Result<Guid>.Failure(dateRangeResult.Error);
            }

            var room = await _roomQueries.GetEntityByIdAsync(request.RoomId, ct);
            if (room is null)
            {
                return Result<Guid>.Failure(RoomErrors.NotFound);
            }

            var guest = await _userManager.FindByIdAsync(request.GuestId.ToString());
            if (guest is null)
            {
                return Result<Guid>.Failure(UserErrors.NotFound);
            }

            var bookingResult = Booking.Domain.Entities.Bookings.Create(
                dateRangeResult.Value,
                request.AdultsCount,
                request.ChildrenCount,
                room,
                guest);

            if (!bookingResult.IsSuccess || bookingResult.Value is null)
            {
                return Result<Guid>.Failure(bookingResult.Error);
            }

            _bookingRepository.Add(bookingResult.Value);
            await unitOfWork.SaveChangesAsync(ct);

            return Result<Guid>.Success(bookingResult.Value.Id);
        }
    }
}
