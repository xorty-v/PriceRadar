using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceRadar.Domain.Entities;
using Constants = PriceRadar.Domain.Constants;

namespace PriceRadar.Infrastructure.Configurations;

internal sealed class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired();

        builder.Property(s => s.Url).IsRequired();

        builder.Property(s => s.IsParserImplemented).HasDefaultValue(false);

        builder.HasMany(s => s.StoreCategories)
            .WithOne(sc => sc.Store)
            .HasForeignKey(sc => sc.StoreId);

        builder.HasData(
            new Store
            {
                Id = Constants.PredefinedIds.Stores.Alta,
                Name = "Alta",
                Url = "https://alta.ge/?sl=en",
                IsParserImplemented = true
            },
            new Store
            {
                Id = Constants.PredefinedIds.Stores.Zoommer,
                Name = "Zoommer",
                Url = "https://zoommer.ge/en",
                IsParserImplemented = true
            },
            new Store
            {
                Id = Constants.PredefinedIds.Stores.EliteElectronic,
                Name = "EliteElectronic",
                Url = "https://ee.ge/",
                IsParserImplemented = true
            });
    }
}