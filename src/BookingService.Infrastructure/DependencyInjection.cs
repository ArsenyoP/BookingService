using Booking.Domain.Entities;
using Booking.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Booking.Application.Interfaces;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Infrastructure.Repositories;
using Booking.Application.Queries;
using Booking.Infrastructure.Queries;
using Booking.Application.Interfaces.IQueries;

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
            services.AddScoped<IListingRepository, ListingRepository>();

            services.AddScoped<IRoomQueries>(sp => new RoomQueries(connectionString!));
            services.AddScoped<IListingQueries>(sp => new ListingQueries(connectionString!));
            services.AddScoped<IBookingQueries>(sp => new BookingQueries(connectionString!));

            services.AddScoped<IRoomQueries>(provider =>
             new RoomQueries(connectionString!));



            return services;
        }
    }
}
