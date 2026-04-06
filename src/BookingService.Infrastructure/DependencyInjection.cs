using Booking.Domain.Entities;
using Booking.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Booking.Application.Interfaces;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Infrastructure.Repositories;

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


            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();



            return services;
        }
    }
}
