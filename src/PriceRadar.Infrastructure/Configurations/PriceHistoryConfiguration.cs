using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceRadar.Domain.Entities;

namespace PriceRadar.Infrastructure.Configurations;

internal sealed class PriceHistoryConfiguration : IEntityTypeConfiguration<PriceHistory>
{
    public void Configure(EntityTypeBuilder<PriceHistory> builder)
    {
        builder.HasKey(ph => ph.Id);

        builder.Property(ph => ph.Price).HasPrecision(18, 2).IsRequired();

        builder.Property(ph => ph.DiscountPrice).HasPrecision(18, 2).IsRequired();

        builder.Property(ph => ph.LastPriceOnUtc).IsRequired();

        builder.HasOne(ph => ph.Offer)
            .WithMany(o => o.PriceHistories)
            .HasForeignKey(ph => ph.OfferId);
    }
}