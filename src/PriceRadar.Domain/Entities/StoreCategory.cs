namespace PriceRadar.Domain.Entities;

public class StoreCategory
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }

    public Store Store { get; set; }
    public Category Category { get; set; }
}