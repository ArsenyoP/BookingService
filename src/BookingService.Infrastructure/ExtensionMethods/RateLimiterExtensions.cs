using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace Booking.Infrastructure.ExtensionMethods
{
    public static class RateLimiterExtensions
    {
        public static IServiceCollection AddRateLimiting(this IServiceCollection services)
        {
            return services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy("fixed", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromSeconds(60),
                            PermitLimit = 30,
                            QueueLimit = 0
                        }));


                options.AddPolicy("auth-limiter", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"auth_{context.Connection.RemoteIpAddress?.ToString() ?? "unknown"}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromSeconds(60),
                            PermitLimit = 10,
                            QueueLimit = 0
                        }));

                options.AddPolicy("write-limiter", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"write_{context.Connection.RemoteIpAddress?.ToString() ?? "unknown"}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromSeconds(60),
                            PermitLimit = 10,
                            QueueLimit = 0
                        }));
            });
        }

    }
}
