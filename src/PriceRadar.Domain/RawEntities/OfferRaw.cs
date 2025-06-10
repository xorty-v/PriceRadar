namespace PriceRadar.Domain.RawEntities;

public record OfferRaw
{
    public Guid CategoryId { get; set; }
    public string Name { get; init; }
    public string Url { get; init; }
    public decimal Price { get; init; }
    public decimal DiscountPrice { get; init; }

    public virtual bool Equals(OfferRaw? other)
    {
        if (other is null) return false;
        return Url == other.Url;
    }

    public override int GetHashCode() => Url.GetHashCode();
}