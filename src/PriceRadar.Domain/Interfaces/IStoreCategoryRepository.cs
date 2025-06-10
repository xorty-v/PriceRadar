using PriceRadar.Domain.Entities;

namespace PriceRadar.Domain.Interfaces;

public interface IStoreCategoryRepository
{
    public Task AddAsync(StoreCategory storeCategory);
    public Task UpdateAsync(StoreCategory storeCategory);
    public Task DeleteAsync(StoreCategory storeCategory);
    public Task<List<StoreCategory>> GetAllAsync();
    public Task<StoreCategory> GetByNameAsync(string name);
    public Task<StoreCategory> GetByIdAsync(Guid id);
    public Task<List<StoreCategory>> GetAllByStoreIdAsync(Guid storeId);
}