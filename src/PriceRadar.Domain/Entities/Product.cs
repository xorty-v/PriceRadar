namespace PriceRadar.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; }

    public Category Category { get; set; }
    public ICollection<Offer> Offers { get; set; }
}