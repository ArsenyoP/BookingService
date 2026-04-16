using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Data.EntitiesConfigurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable("Rooms");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.PricePerNight)
                .HasColumnType("decimal(18,2)");


            builder.HasOne<Listing>("_listing")
               .WithMany()
               .HasForeignKey(x => x.ListingId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Amenities)
                .WithMany()
                .UsingEntity(j => j.ToTable("RoomAmenities"));

            builder.Navigation(x => x.Amenities)
            .HasField("_amenity")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(x => x.Title);
        }
    }
}
