using Booking.Application.Interfaces;
using Booking.Application.Interfaces.IQueries;
using Booking.Application.Interfaces.Services;
using Booking.Application.Queries;
using Booking.Domain.Entities;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Domain.Interfaces.Services;
using Booking.Infrastructure.Data;
using Booking.Infrastructure.Queries;
using Booking.Infrastructure.Repositories;
using Booking.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
             options.UseSqlServer(connectionString));

            services.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<AppDbContext>();

            services.AddSingleton<ITokenService, TokenService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IListingRepository, ListingRepository>();
            services.AddScoped<IAmenityRepository, AmenityRepository>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IRoomQueries>(sp => new RoomQueries(connectionString!));
            services.AddScoped<IListingQueries>(sp => new ListingQueries(connectionString!));
            services.AddScoped<IBookingQueries>(sp => new BookingQueries(connectionString!));
            services.AddScoped<IAmenityQueries>(sp => new AmenityQueries(connectionString!));





            return services;
        }
    }
}
