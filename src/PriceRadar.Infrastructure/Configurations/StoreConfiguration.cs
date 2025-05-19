using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceRadar.Domain;
using PriceRadar.Domain.Entities;

namespace PriceRadar.Infrastructure.Configurations;

internal sealed class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired();

        builder.Property(s => s.Url).IsRequired();

        builder.HasData(
            new Store { Id = Constants.PredefinedIds.Stores.Alta, Name = "Alta", Url = "https://alta.ge/?sl=en" },
            new Store { Id = Constants.PredefinedIds.Stores.Zoommer, Name = "Zoommer", Url = "https://zoommer.ge/en" },
            new Store
            {
                Id = Constants.PredefinedIds.Stores.EliteElectronic, Name = "EliteElectronic", Url = "https://ee.ge/"
            });
    }
}