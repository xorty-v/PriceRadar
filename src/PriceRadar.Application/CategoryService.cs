using PriceRadar.Domain.Entities;
using PriceRadar.Domain.Interfaces;

namespace PriceRadar.Application;

internal sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task AddCategoryAsync(string name)
    {
        // TODO: УБРАТЬ ВОЗМОЖНОСТЬ ХРАНИТЬ С ОДИНАКОВЫМИ ИМЕНАМИ 

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = name
        };

        await _categoryRepository.AddAsync(category);
    }

    public async Task UpdateCategoryAsync(Guid id, string name)
    {
        // TODO: УБРАТЬ ВОЗМОЖНОСТЬ НА ОБНОВЛНЕНИЯ НА УЖЕ СУЩЕСТВУЮЩИЕ НАЗВАНИЕ
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
        {
            return;
        }

        category.Name = name;

        await _categoryRepository.UpdateAsync(category);
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
        {
            return;
        }

        await _categoryRepository.DeleteAsync(category);
    }

    public Task<List<Category>> GetAllCategoriesAsync()
    {
        return _categoryRepository.GetAllAsync();
    }
}