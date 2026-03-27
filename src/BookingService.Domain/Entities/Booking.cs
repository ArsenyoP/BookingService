using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Domain.Common;
using Booking.Domain.Enums;
using Booking.Domain.Errors;
using Booking.Domain.ValueObjects;

namespace Booking.Domain.Entities
{
    public class Booking : Entity
    {
        public Guid RoomId { get; private set; }
        public Guid GuestId { get; private set; }

        public DateRange Period { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        public decimal PricePerNight { get; private set; }
        public decimal TotalPrice { get; private set; }

        public int AdultsCount { get; private set; }
        public int ChildrenCount { get; private set; }
        public BookingStatus Status { get; private set; }

        private Booking() { }

        private Booking(Guid roomId, Guid guestId, DateRange period, int adults, int children, decimal pricePerNight)
        {
            Id = Guid.NewGuid();
            RoomId = roomId;
            GuestId = guestId;

            Period = period;
            CreatedAtUtc = DateTime.UtcNow;

            PricePerNight = pricePerNight;
            TotalPrice = pricePerNight * period.TotalNights;

            AdultsCount = adults;
            ChildrenCount = children;

            Status = BookingStatus.Pending;
        }


        public static Result<Booking> Create(DateRange period,
            int numberOfAdults, int numberOfChildren,
            Room room, User guest)
        {

            if (numberOfAdults <= 0)
                return Result<Booking>.Failure(BookingErrors.AtLeastOneAdultRequired);

            if (numberOfChildren < 0)
                return Result<Booking>.Failure(BookingErrors.NegativeChildrenCount);

            if (numberOfAdults + numberOfChildren > room.AdultsCapacity + room.ChildrenCapacity)
                return Result<Booking>.Failure(BookingErrors.ExceedsCapacity);

            if (!guest.IsActive)
                return Result<Booking>.Failure(UserErrors.AccountInactive);

            var booking = new Booking(room.Id, guest.Id, period, numberOfAdults, numberOfChildren, room.PricePerNight);
            booking.Status = BookingStatus.Confirmed;

            return Result<Booking>.Success(booking);
        }

        public Result<RefundValue> Cancel()
        {
            //TODO: Add payment refund
            //TODO: Fix race condition 
            if (Period.StartDate <= DateOnly.FromDateTime(DateTime.UtcNow))
            {
                return Result<RefundValue>.Failure(BookingErrors.CannotCancelStartedBooking);
            }

            if (Status != BookingStatus.Confirmed && Status != BookingStatus.Pending)
                return Result<RefundValue>.Failure(BookingErrors.CannotCancel);

            int daysBeforeStart = Period.StartDate.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;

            double percentToRefund = daysBeforeStart switch
            {
                >= 7 => 100,
                >= 3 => 75,
                >= 1 => 50,
                _ => 0
            };

            var refundValue = new RefundValue(TotalPrice, percentToRefund);
            Status = BookingStatus.Cancelled;

            return Result<RefundValue>.Success(refundValue);
        }
    }
}
