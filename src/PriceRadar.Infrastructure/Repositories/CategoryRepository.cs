using Microsoft.EntityFrameworkCore;
using PriceRadar.Domain.Entities;
using PriceRadar.Domain.Interfaces;

namespace PriceRadar.Infrastructure.Repositories;

internal sealed class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CategoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Category category)
    {
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        _dbContext.Categories.Update(category);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Category category)
    {
        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Category> GetByIdAsync(Guid id)
    {
        return await _dbContext.Categories.SingleOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category> GetByNameAsync(string name)
    {
        return await _dbContext.Categories.SingleOrDefaultAsync(c => c.Name == name);
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _dbContext.Categories.ToListAsync();
    }
}