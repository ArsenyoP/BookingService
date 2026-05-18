using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Data.EntitiesConfigurations
{
    public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.Content)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(x => x.OccurredOnUtc)
                .IsRequired();

            builder.Property(x => x.ProcessedOnUtc)
                .IsRequired(false);

            builder.Property(x => x.Error)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            builder.HasIndex(x => new { x.ProcessedOnUtc, x.OccurredOnUtc })
                .HasDatabaseName("IX_OutboxMessages_ProcessedOnUtc_OccurredOnUtc");
        }
    }
}
