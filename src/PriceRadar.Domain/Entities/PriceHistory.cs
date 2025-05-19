namespace PriceRadar.Domain.Entities;

public class PriceHistory
{
    public Guid Id { get; set; }
    public Guid OfferId { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountPrice { get; set; }
    public DateTime LastPriceOnUtc { get; set; }

    public Offer Offer { get; set; }
}