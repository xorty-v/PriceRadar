using PriceRadar.Domain.Entities;

namespace PriceRadar.Application;

public interface IStoreCategoryService
{
    public Task AddAsync(Guid storeId, Guid categoryId, string name, string url);
    public Task UpdateAsync(Guid id, Guid storeId, Guid categoryId, string name, string url);
    public Task DeleteAsync(Guid id);
    public Task<List<StoreCategory>> GetAllAsync();
    public Task<List<StoreCategory>> GetByStoreAsync(Guid storeId);
}