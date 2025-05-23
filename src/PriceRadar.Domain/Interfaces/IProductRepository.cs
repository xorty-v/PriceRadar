using PriceRadar.Domain.Entities;

namespace PriceRadar.Domain.Interfaces;

public interface IProductRepository
{
    public Task AddProductsAsync(List<Product> products);
    public Task<List<Product>> GetProductsByCategoryAsync(Guid categoryId);
}