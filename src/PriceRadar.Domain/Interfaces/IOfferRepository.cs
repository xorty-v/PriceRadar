using PriceRadar.Domain.Entities;

namespace PriceRadar.Domain.Interfaces;

public interface IOfferRepository
{
    public Task AddOffersAsync(List<Offer> offers);
    public Task UpdateOffersAsync(List<Offer> offers);
    public Task<List<Offer>> GetOffersByStoreAsync(Guid storeId);
    public Task<List<Offer>> GetOffersByCategoryAsync(Guid categoryId);
}