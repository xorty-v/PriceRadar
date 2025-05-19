using PriceRadar.Domain.Entities;

namespace PriceRadar.Domain.Interfaces;

public interface IStoreRepository
{
    public Task<Store> GetStoreByNameAsync(string name);
}