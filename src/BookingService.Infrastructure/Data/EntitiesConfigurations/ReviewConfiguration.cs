using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Data.EntitiesConfigurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews", t =>
            {
                t.HasCheckConstraint("CK_Review_Score_Range",
                    "[Score] >= 1 AND [Score] <= 5");

                t.HasCheckConstraint("CK_Review_Text_Range",
                    "LEN([Text]) > 10 AND LEN([Text]) < 1000");

                t.HasCheckConstraint("CK_Review_TargetType",
                    "[TargetType] IN ('Room', 'Listing')");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Text)
                .IsRequired();

            builder.Property(x => x.Score)
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasDefaultValue(Guid.Empty)
                .IsRequired();

            builder.Property(x => x.TargetId)
                .HasDefaultValue(Guid.Empty)
                .IsRequired();

            builder.Property(x => x.IsEdited)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(x => x.TargetType)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.Score);

            builder.HasIndex(x => new { x.TargetId, x.CreatedAt });
        }
    }
}
