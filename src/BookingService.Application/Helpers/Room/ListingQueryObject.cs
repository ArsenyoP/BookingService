using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Application.Helpers.Room
{
    public sealed record ListingQueryObject(
        string Title = "",
        string Country = "",
        string City = "",
        string Type = "",
        [FromQuery]
        List<string>? AmenityNames = null,
        int PageSize = 10,
        int Page = 1)
    {
        public static ValueTask<ListingQueryObject?> BindAsync(HttpContext context)
        {
            var query = context.Request.Query;

            string? title = query["Title"];
            string? country = query["Country"];
            string? city = query["City"];
            string? type = query["Type"];

            int pageSize = int.TryParse(query["PageSize"], out var ps)
                ? ps
                : 10;

            int page = int.TryParse(query["Page"], out var p)
                ? p
                : 1;

            var amenityNames = query["AmenityNames"]
                .ToList();

            var result = new ListingQueryObject(
                string.IsNullOrWhiteSpace(title) ? null : title,
                string.IsNullOrWhiteSpace(country) ? null : country,
                string.IsNullOrWhiteSpace(city) ? null : city,
                string.IsNullOrWhiteSpace(type) ? null : type,
                amenityNames.Count == 0 ? null : amenityNames,
                pageSize,
                page);

            return ValueTask.FromResult<ListingQueryObject?>(result);
        }
    };
}
