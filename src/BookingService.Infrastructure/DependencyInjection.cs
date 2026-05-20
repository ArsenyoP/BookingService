using Booking.Application.Interfaces;
using Booking.Application.Interfaces.IQueries;
using Booking.Application.Interfaces.Services;
using Booking.Application.Queries;
using Booking.Domain.Entities;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Domain.Interfaces.Services;
using Booking.Infrastructure.BackgroundJobs;
using Booking.Infrastructure.Data;
using Booking.Infrastructure.Interceptors;
using Booking.Infrastructure.Queries;
using Booking.Infrastructure.Repositories;
using Booking.Infrastructure.Seeding;
using Booking.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Web.API.Models.Settings;

namespace Booking.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration, IWebHostEnvironment environment)
        {
            string connectionString;

            if (environment.IsDevelopment())
            {
                connectionString = configuration.GetConnectionString("LocalConnections")!;
            }
            else
            {
                connectionString = configuration.GetConnectionString("CloudConnection")!;
            }

            services.AddSingleton<ConvertDomainEventToOutboxMessageInterceptor>();

            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });

                var interceptor = sp.GetRequiredService<ConvertDomainEventToOutboxMessageInterceptor>();
                options.AddInterceptors(interceptor);
            });

            services.AddQuartz(configure =>
            {
                var jobKey = new JobKey(nameof(ProcessOutboxMessageJob));

                configure
                    .AddJob<ProcessOutboxMessageJob>(jobKey)
                    .AddTrigger(
                        trigger =>
                            trigger.ForJob(jobKey)
                                .WithSimpleSchedule(
                                    schedule =>
                                        schedule.WithIntervalInSeconds(10)
                                            .RepeatForever()));

            });

            services.AddQuartzHostedService();


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
                options.InstanceName = "Distributed_Cache_";
            });

            services.AddStackExchangeRedisOutputCache(options =>
            {
                options.Configuration = configuration["Redis:Connection"];
                options.InstanceName = "Output_Cache_";
            });


            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IListingRepository, ListingRepository>();
            services.AddScoped<IAmenityRepository, AmenityRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<DataSeeder>();

            services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));
            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IRoomQueries>(sp => new RoomQueries(connectionString!));
            services.AddScoped<IListingQueries>(sp => new ListingQueries(connectionString!));
            services.AddScoped<IBookingQueries>(sp => new BookingQueries(connectionString!));
            services.AddScoped<IAmenityQueries>(sp => new AmenityQueries(connectionString!));
            services.AddScoped<IReviewQueries>(sp => new ReviewQueries(connectionString!));




            return services;
        }
    }
}
