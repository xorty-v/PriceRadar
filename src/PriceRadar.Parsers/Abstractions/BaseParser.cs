using PriceRadar.Domain.RawEntities;

namespace PriceRadar.Parsers.Abstractions;

public abstract class BaseParser
{
    protected abstract string CategoryUrl { get; }
    protected abstract string ProductUrl { get; }

    public abstract Task<List<CategoryRaw>> ParseCategoriesAsync();
    public abstract Task<HashSet<OfferRaw>> ParseOffersAsync(List<CategoryRaw> categories);
}