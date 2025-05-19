using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceRadar.Domain.Entities;

namespace PriceRadar.Infrastructure.Configurations;

internal sealed class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Name).IsRequired();

        builder.HasIndex(o => o.Url).IsUnique();

        builder.Property(o => o.Category).IsRequired();

        builder.HasOne(o => o.Product)
            .WithMany(p => p.Offers)
            .HasForeignKey(o => o.ProductId);

        builder.HasOne(o => o.Store)
            .WithMany(s => s.Offers)
            .HasForeignKey(o => o.StoreId);
    }
}