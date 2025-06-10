using Microsoft.EntityFrameworkCore;
using PriceRadar.Domain.Entities;
using PriceRadar.Domain.Interfaces;

namespace PriceRadar.Infrastructure.Repositories;

internal sealed class StoreRepository : IStoreRepository
{
    private readonly ApplicationDbContext _dbContext;

    public StoreRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddStoreAsync(Store store)
    {
        _dbContext.Stores.Add(store);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateStoreAsync(Store store)
    {
        _dbContext.Stores.Update(store);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteStoreAsync(Store store)
    {
        _dbContext.Stores.Remove(store);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Store> GetByIdAsync(Guid id)
    {
        return await _dbContext.Stores.SingleOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Store>> GetAllAsync()
    {
        return await _dbContext.Stores.Where(s => s.IsParserImplemented).ToListAsync();
    }

    public async Task<Store> GetStoreByNameAsync(string name)
    {
        return await _dbContext.Stores.SingleOrDefaultAsync(s => s.Name == name);
    }
}