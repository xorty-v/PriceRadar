using PriceRadar.Domain.Entities;

namespace PriceRadar.Domain.Interfaces;

public interface ICategoryRepository
{
    public Task AddAsync(Category category);
    public Task UpdateAsync(Category category);
    public Task DeleteAsync(Category category);
    public Task<Category> GetByIdAsync(Guid id);
    public Task<Category> GetByNameAsync(string name);
    public Task<List<Category>> GetAllAsync();
}