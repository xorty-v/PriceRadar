using PriceRadar.Domain.Entities;
using PriceRadar.Domain.Interfaces;

namespace PriceRadar.Application;

internal sealed class StoreCategoryService : IStoreCategoryService
{
    private readonly IStoreCategoryRepository _storeCategoryRepository;

    public StoreCategoryService(IStoreCategoryRepository storeCategoryRepository)
    {
        _storeCategoryRepository = storeCategoryRepository;
    }

    public async Task AddAsync(Guid storeId, Guid categoryId, string name, string url)
    {
        var storeCategory = new StoreCategory
        {
            Id = Guid.NewGuid(),
            StoreId = storeId,
            CategoryId = categoryId,
            Name = name,
            Url = url
        };

        await _storeCategoryRepository.AddAsync(storeCategory);
    }

    public async Task UpdateAsync(Guid id, Guid storeId, Guid categoryId, string name, string url)
    {
        var storeCategory = await _storeCategoryRepository.GetByIdAsync(id);

        if (storeCategory == null)
        {
            return;
        }

        storeCategory = new StoreCategory
        {
            StoreId = storeId,
            CategoryId = categoryId,
            Name = name,
            Url = url
        };

        await _storeCategoryRepository.UpdateAsync(storeCategory);
    }

    public async Task DeleteAsync(Guid id)
    {
        var storeCategory = await _storeCategoryRepository.GetByIdAsync(id);

        if (storeCategory == null)
        {
            return;
        }

        await _storeCategoryRepository.DeleteAsync(storeCategory);
    }

    public async Task<List<StoreCategory>> GetAllAsync()
    {
        return await _storeCategoryRepository.GetAllAsync();
    }


    public async Task<List<StoreCategory>> GetByStoreAsync(Guid storeId)
    {
        return await _storeCategoryRepository.GetAllByStoreIdAsync(storeId);
    }
}