using Microsoft.EntityFrameworkCore;
using PriceRadar.Domain.Entities;
using PriceRadar.Domain.Interfaces;

namespace PriceRadar.Infrastructure.Repositories;

internal sealed class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddProductsAsync(List<Product> products)
    {
        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Product>> GetProductsByCategoryAsync(Guid categoryId)
    {
        return await _dbContext.Products
            .Where(o => o.CategoryId == categoryId)
            .ToListAsync();
    }
}