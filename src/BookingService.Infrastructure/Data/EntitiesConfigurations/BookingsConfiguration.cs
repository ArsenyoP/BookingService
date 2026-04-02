using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Data.EntitiesConfigurations
{
    public class BookingsConfiguration : IEntityTypeConfiguration<Bookings>
    {
        public void Configure(EntityTypeBuilder<Bookings> builder)
        {
            builder.ToTable("Bookingss");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.GuestId).IsRequired();

            builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

            builder.Property(x => x.PricePerNight)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

            builder.Property(x => x.TotalPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

            builder.Property(x => x.AdultsCount)
            .IsRequired();

            builder.Property(x => x.ChildrenCount)
            .IsRequired();


            builder.OwnsOne(x => x.Period, periodBuilder =>
            {
                periodBuilder.Property(p => p.StartDate)
                    .HasColumnName("StartDate")
                    .IsRequired();

                periodBuilder.Property(p => p.EndDate)
                    .HasColumnName("EndDate")
                    .IsRequired();

                periodBuilder.HasIndex(p => new { p.StartDate, p.EndDate });
            });


            builder.HasOne<Room>("Room")
                .WithMany()
                .HasForeignKey(x => x.RoomId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>("Guest")
                .WithMany()
                .HasForeignKey(x => x.GuestId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.HasIndex(x => x.GuestId);
            builder.HasIndex(x => x.RoomId);

        }
    }
}
