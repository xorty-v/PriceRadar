using PriceRadar.Domain.Entities;

namespace PriceRadar.Domain.Interfaces;

public interface IStoreCategoryRepository
{
    public Task<List<StoreCategory>> GetCategoriesByStoreAsync(Guid storeId);
}