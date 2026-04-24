using Booking.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        public DbSet<Bookings> Bookings { get; set; }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<Room> Rooms { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var adminRoleId = new Guid("3474648c-112d-460c-bc8a-61be046b3078");
            var userRoleId = new Guid("eb4b3260-05fb-470c-af9f-b9e9b9bcd85e");
            var guestRoleId = new Guid("a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d");

            builder.Entity<IdentityRole<Guid>>().HasData(
                new IdentityRole<Guid>
                {
                    Id = userRoleId,
                    Name = "User",
                    NormalizedName = "USER"
                },
                new IdentityRole<Guid>
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole<Guid>
                {
                    Id = guestRoleId,
                    Name = "Guest",
                    NormalizedName = "GUEST"
                }
            );

            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
