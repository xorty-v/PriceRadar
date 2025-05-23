using PriceRadar.Domain.Entities;

namespace PriceRadar.Domain.Interfaces;

public interface ICategoryRepository
{
    public Task<IEnumerable<Category>> GetAllCategoriesAsync();
}