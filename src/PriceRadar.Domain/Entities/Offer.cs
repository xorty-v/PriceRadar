namespace PriceRadar.Domain.Entities;

public class Offer
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid StoreId { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public bool IsAvailable { get; set; }

    public Category Category { get; set; }
    public Product? Product { get; set; }
    public Store Store { get; set; }
    public ICollection<PriceHistory> PriceHistories { get; set; }
}