using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Data
{
    public class ListingConfiguration : IEntityTypeConfiguration<Listing>
    {
        public void Configure(EntityTypeBuilder<Listing> builder)
        {
            builder.ToTable("Listings");
            builder.HasKey(x => x.Id);

            builder.Property(l => l.Title)
            .HasMaxLength(100)
            .IsRequired();

            builder.Property(l => l.Description)
                .HasMaxLength(1000)
                .IsRequired();

            builder.OwnsOne(x => x.Address, addressBuilder =>
            {
                addressBuilder.Property(a => a.Country).HasMaxLength(50).IsRequired();
                addressBuilder.Property(a => a.City).HasMaxLength(50).IsRequired();
                addressBuilder.Property(a => a.Street).HasMaxLength(100).IsRequired();
            });


            builder.Property(l => l.ListingType)
            .HasConversion<string>()
            .HasMaxLength(20);


            builder.Navigation(x => x.Amenities)
            .UsePropertyAccessMode(PropertyAccessMode.Field);


            builder.HasMany(x => x.Amenities)
                .WithMany()
                .UsingEntity(j => j.ToTable("ListingAmenities"));


            builder.HasIndex(x => x.Title);
        }
    }
}
