using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Data.EntitiesConfigurations
{
    public class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
    {
        public void Configure(EntityTypeBuilder<Amenity> builder)
        {
            builder.ToTable("Amenities");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

            builder.Property(x => x.Category)
            .HasMaxLength(50)
            .IsRequired();

            builder.HasIndex(x => new { x.Name, x.Category }).IsUnique();
        }
    }
}
