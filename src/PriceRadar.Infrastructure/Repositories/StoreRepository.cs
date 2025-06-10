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

    public async Task<List<Store>> GetStoresAsync()
    {
        return await _dbContext.Stores.Where(s => s.IsParserImplemented).ToListAsync();
    }
}