using Booking.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Domain.Errors;

namespace Booking.Domain.ValueObjects
{
    public record DateRange
    {
        public DateOnly StartDate { get; init; }
        public DateOnly EndDate { get; init; }

        public int TotalNights => EndDate.Day - StartDate.Day;

        public DateRange(DateOnly startDate, DateOnly endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public Result<DateRange> Create(DateOnly startDate, DateOnly endDate)
        {
            if (startDate < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                return Result<DateRange>.Failure(DateRangeErrors.PastStart);
            }

            if (endDate <= startDate)
            {
                return Result<DateRange>.Failure(DateRangeErrors.InvalidEnd);
            }
            return Result<DateRange>.Success(new DateRange(startDate, endDate));
        }

        public bool OverlapsWith(DateRange other)
        {
            return StartDate < other.EndDate && EndDate > other.StartDate;
        }
    }
}
