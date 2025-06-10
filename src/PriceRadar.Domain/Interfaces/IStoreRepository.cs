using PriceRadar.Domain.Entities;

namespace PriceRadar.Domain.Interfaces;

public interface IStoreRepository
{
    public Task AddStoreAsync(Store store);
    public Task UpdateStoreAsync(Store store);
    public Task DeleteStoreAsync(Store store);
    public Task<Store> GetStoreByNameAsync(string name);
    public Task<Store> GetByIdAsync(Guid id);
    public Task<List<Store>> GetAllAsync();
}