using PriceRadar.Domain.Entities;
using PriceRadar.Domain.Interfaces;

namespace PriceRadar.Application;

internal sealed class StoreService : IStoreService
{
    private readonly IStoreRepository _storeRepository;

    public StoreService(IStoreRepository storeRepository)
    {
        _storeRepository = storeRepository;
    }

    public async Task AddAsync(string name, string url)
    {
        var store = new Store
        {
            Id = Guid.NewGuid(),
            Name = name,
            Url = url
        };

        await _storeRepository.AddStoreAsync(store);
    }

    public async Task UpdateAsync(Guid id, string name, string url)
    {
        var store = await _storeRepository.GetByIdAsync(id);

        if (store == null)
        {
            return;
        }

        store = new Store
        {
            Name = name,
            Url = url
        };

        await _storeRepository.UpdateStoreAsync(store);
    }

    public async Task DeleteAsync(Guid id)
    {
        var store = await _storeRepository.GetByIdAsync(id);

        if (store == null)
        {
            return;
        }

        await _storeRepository.DeleteStoreAsync(store);
    }

    public async Task<List<Store>> GetAllAsync()
    {
        return await _storeRepository.GetAllAsync();
    }
}