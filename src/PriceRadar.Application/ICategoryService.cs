using PriceRadar.Domain.Entities;

namespace PriceRadar.Application;

public interface ICategoryService
{
    public Task AddCategoryAsync(string name);
    public Task UpdateCategoryAsync(Guid id, string name);
    public Task DeleteCategoryAsync(Guid id);
    public Task<List<Category>> GetAllCategoriesAsync();
}