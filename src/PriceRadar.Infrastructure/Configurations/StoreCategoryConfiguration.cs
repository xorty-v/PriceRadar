using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceRadar.Domain.Entities;

namespace PriceRadar.Infrastructure.Configurations;

public class StoreCategoryConfiguration : IEntityTypeConfiguration<StoreCategory>
{
    public void Configure(EntityTypeBuilder<StoreCategory> builder)
    {
        builder.HasKey(sc => sc.Id);

        builder.Property(sc => sc.Name).IsRequired();

        builder.Property(sc => sc.Url).IsRequired();

        builder.HasOne(sc => sc.Store)
            .WithMany(s => s.StoreCategories)
            .HasForeignKey(sc => sc.StoreId);

        builder.HasOne(sc => sc.Category)
            .WithMany()
            .HasForeignKey(sc => sc.CategoryId);
    }
}