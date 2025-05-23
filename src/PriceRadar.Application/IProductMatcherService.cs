namespace PriceRadar.Application;

public interface IProductMatcherService
{
    public Task MatchOffersToProductsAsync();
}