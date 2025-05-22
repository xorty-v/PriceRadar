using PriceRadar.Application.Abstractions;
using PriceRadar.Application.Abstractions.Parsers;
using PriceRadar.Domain.Entities;
using PriceRadar.Domain.Interfaces;

namespace PriceRadar.Application;

public class ParserService : IParserService
{
    private readonly ICategoryMapperService _categoryMapperService;
    private readonly IOfferRepository _offerRepository;
    private readonly IParserFactory _parserFactory;
    private readonly IStoreRepository _storeRepository;

    public ParserService(
        IParserFactory parserFactory,
        IOfferRepository offerRepository,
        IStoreRepository storeRepository,
        ICategoryMapperService categoryMapperService)
    {
        _parserFactory = parserFactory;
        _offerRepository = offerRepository;
        _storeRepository = storeRepository;
        _categoryMapperService = categoryMapperService;
    }

    public async Task RunAllParsers()
    {
        var storeTypes = Enum.GetValues<StoreType>().ToList();

        foreach (var storeType in storeTypes)
        {
            var parser = _parserFactory.CreateParser(storeType);
            var rawOffers = await parser.ParseAsync();

            var store = await _storeRepository.GetStoreByNameAsync(storeType.ToString());
            var existingOffers = await _offerRepository.GetOffersByStoreAsync(store.Id);

            var existingDict = existingOffers.ToDictionary(o => o.Url, o => o);
            var incomingUrls = new HashSet<string>();

            var newOffers = new List<Offer>();
            var offersToUpdate = new List<Offer>();

            foreach (var rawOffer in rawOffers)
            {
                incomingUrls.Add(rawOffer.Url);

                if (existingDict.TryGetValue(rawOffer.Url, out var existing))
                {
                    var last = existing.PriceHistories.FirstOrDefault();
                    if (last == null || last.Price != rawOffer.Price || last.DiscountPrice != rawOffer.DiscountPrice)
                    {
                        existing.PriceHistories.Add(new PriceHistory
                        {
                            Price = rawOffer.Price,
                            DiscountPrice = rawOffer.DiscountPrice,
                            LastPriceOnUtc = DateTime.UtcNow
                        });
                    }

                    existing.Name = rawOffer.Name;
                    existing.IsAvailable = true;
                    offersToUpdate.Add(existing);
                }
                else
                {
                    var offer = new Offer
                    {
                        Url = rawOffer.Url,
                        Name = rawOffer.Name,
                        CategoryId = _categoryMapperService.Map(storeType, rawOffer.Category),
                        StoreId = store.Id,
                        IsAvailable = true,
                        PriceHistories = new List<PriceHistory>
                        {
                            new()
                            {
                                Price = rawOffer.Price,
                                DiscountPrice = rawOffer.DiscountPrice,
                                LastPriceOnUtc = DateTime.UtcNow
                            }
                        }
                    };

                    newOffers.Add(offer);
                }
            }

            foreach (var offer in existingOffers.Where(o => !incomingUrls.Contains(o.Url) && o.IsAvailable))
            {
                offer.IsAvailable = false;
                offersToUpdate.Add(offer);
            }

            if (newOffers.Any()) await _offerRepository.AddOffersAsync(newOffers);
            if (offersToUpdate.Any()) await _offerRepository.UpdateOffersAsync(offersToUpdate);
        }
    }
}