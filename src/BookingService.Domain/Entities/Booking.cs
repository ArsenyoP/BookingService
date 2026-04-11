using Booking.Domain.Common;
using Booking.Domain.Enums;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces;
using Booking.Domain.ValueObjects;

namespace Booking.Domain.Entities
{
    public class Bookings : Entity
    {
        public Guid RoomId { get; private set; }
        private Room Room { get; set; }

        public Guid GuestId { get; private set; }
        private User Guest { get; set; }

        public DateRange Period { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        public decimal PricePerNight { get; private set; }
        public decimal TotalPrice { get; private set; }
        public int TotalNights { get; private set; }

        public int AdultsCount { get; private set; }
        public int ChildrenCount { get; private set; }
        public BookingStatus Status { get; private set; }

        private Bookings() { }

        private Bookings(Guid roomId, Guid guestId, DateRange period, int adults, int children, decimal pricePerNight)
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


        public static Result<Bookings> Create(DateRange period,
            int numberOfAdults, int numberOfChildren,
            Room room, User guest)
        {

            if (numberOfAdults <= 0)
                return Result<Bookings>.Failure(BookingErrors.AtLeastOneAdultRequired);

            if (numberOfChildren < 0)
                return Result<Bookings>.Failure(BookingErrors.NegativeChildrenCount);

            if (numberOfAdults + numberOfChildren > room.AdultsCapacity + room.ChildrenCapacity)
                return Result<Bookings>.Failure(BookingErrors.ExceedsCapacity);

            if (!guest.IsActive)
                return Result<Bookings>.Failure(UserErrors.AccountInactive);

            var booking = new Bookings(room.Id, guest.Id, period, numberOfAdults, numberOfChildren, room.PricePerNight);
            booking.Status = BookingStatus.Confirmed;

            return Result<Bookings>.Success(booking);
        }

        public Result<RefundValue> Cancel(DateTime nowUtc, IRefundPolicy refundPolicy)
        {
            //TODO: Add payment refund
            //TODO: Fix race condition 
            if (Period.StartDate <= DateOnly.FromDateTime(nowUtc))
            {
                return Result<RefundValue>.Failure(BookingErrors.CannotCancelStartedBooking);
            }

            if (Status != BookingStatus.Confirmed && Status != BookingStatus.Pending)
                return Result<RefundValue>.Failure(BookingErrors.CannotCancel);




            RefundValue refundValue = refundPolicy.CalculateRefund(this, nowUtc);
            Status = BookingStatus.Cancelled;

            return Result<RefundValue>.Success(refundValue);
        }
    }
}
