using PriceRadar.Domain.Entities;

namespace PriceRadar.Application;

public interface IStoreService
{
    public Task AddAsync(string name, string url);
    public Task UpdateAsync(Guid id, string name, string url);
    public Task DeleteAsync(Guid id);
    public Task<List<Store>> GetAllAsync();
}