namespace PriceRadar.Domain.Entities;

public class Store
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public bool IsParserImplemented { get; set; }

    public ICollection<Offer> Offers { get; set; }
    public ICollection<StoreCategory> StoreCategories { get; set; }
}