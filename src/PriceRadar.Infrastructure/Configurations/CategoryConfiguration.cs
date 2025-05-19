using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceRadar.Domain;
using PriceRadar.Domain.Entities;

namespace PriceRadar.Infrastructure.Configurations;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired();

        builder.HasData(
            new Category { Id = Constants.PredefinedIds.Categories.Laptops, Name = "Laptops", },
            new Category { Id = Constants.PredefinedIds.Categories.Smartphones, Name = "Smartphones" },
            new Category { Id = Constants.PredefinedIds.Categories.Monitors, Name = "Monitors", },
            new Category { Id = Constants.PredefinedIds.Categories.Headphones, Name = "Headphones" },
            new Category { Id = Constants.PredefinedIds.Categories.Keyboards, Name = "Keyboards" },
            new Category { Id = Constants.PredefinedIds.Categories.Mouses, Name = "Mouses", });
    }
}