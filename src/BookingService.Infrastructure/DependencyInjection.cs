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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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

            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.User.RequireUniqueEmail = true;
            })
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddSignInManager();


            services.AddAuthorization();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JWT:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["JWT:Audience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(configuration["JWT:SigningKey"])
                    )
                    };
                });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["Redis:Connection"];
            });


            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IListingRepository, ListingRepository>();
            services.AddScoped<IAmenityRepository, AmenityRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICacheService, CacheService>();

            services.AddScoped<IRoomQueries>(sp => new RoomQueries(connectionString!));
            services.AddScoped<IListingQueries>(sp => new ListingQueries(connectionString!));
            services.AddScoped<IBookingQueries>(sp => new BookingQueries(connectionString!));
            services.AddScoped<IAmenityQueries>(sp => new AmenityQueries(connectionString!));





            return services;
        }
    }
}
