using Microsoft.EntityFrameworkCore;
using PriceRadar.Domain.Entities;
using PriceRadar.Domain.Interfaces;

namespace PriceRadar.Infrastructure.Repositories;

internal sealed class OfferRepository : IOfferRepository
{
    private readonly ApplicationDbContext _dbContext;

    public OfferRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddOffersAsync(List<Offer> offers)
    {
        _dbContext.Offers.AddRange(offers);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateOffersAsync(List<Offer> offers)
    {
        _dbContext.Offers.UpdateRange(offers);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Offer>> GetOffersByStoreAsync(Guid storeId)
    {
        return await _dbContext.Offers
            .Include(o => o.PriceHistories.OrderByDescending(ph => ph.LastPriceOnUtc).Take(1))
            .Where(o => o.StoreId == storeId)
            .ToListAsync();
    }
}