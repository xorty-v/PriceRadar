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

    public async Task AddAsync(StoreCategory storeCategory)
    {
        _dbContext.StoreCategories.Add(storeCategory);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(StoreCategory storeCategory)
    {
        _dbContext.StoreCategories.Update(storeCategory);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(StoreCategory storeCategory)
    {
        _dbContext.StoreCategories.Remove(storeCategory);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<StoreCategory>> GetAllAsync()
    {
        return await _dbContext.StoreCategories.ToListAsync();
    }

    public async Task<StoreCategory> GetByNameAsync(string name)
    {
        return await _dbContext.StoreCategories.SingleOrDefaultAsync(sc => sc.Name == name);
    }

    public async Task<StoreCategory> GetByIdAsync(Guid id)
    {
        return await _dbContext.StoreCategories.SingleOrDefaultAsync(sc => sc.Id == id);
    }

    public async Task<List<StoreCategory>> GetAllByStoreIdAsync(Guid storeId)
    {
        return await _dbContext.StoreCategories
            .AsNoTracking()
            .Where(sc => sc.StoreId == storeId)
            .ToListAsync();
    }
}