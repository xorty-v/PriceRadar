using Microsoft.EntityFrameworkCore;
using PriceRadar.Domain.Entities;
using PriceRadar.Domain.Interfaces;

namespace PriceRadar.Infrastructure.Repositories;

public class StoreCategoryRepository : IStoreCategoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public StoreCategoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<StoreCategory>> GetCategoriesByStoreAsync(Guid storeId)
    {
        return await _dbContext.StoreCategories
            .AsNoTracking()
            .Where(sc => sc.StoreId == storeId)
            .ToListAsync();
    }
}