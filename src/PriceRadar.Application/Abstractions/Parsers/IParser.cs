using PriceRadar.Domain.RawEntities;

namespace PriceRadar.Application.Abstractions.Parsers;

public interface IParser
{
    public Task<List<OfferRaw>> ParseAsync();
}