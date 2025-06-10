using System.Text.Json;
using System.Text.RegularExpressions;
using PriceRadar.Application.Abstractions.Loaders;
using PriceRadar.Application.Abstractions.Parsers;
using PriceRadar.Domain;
using PriceRadar.Domain.Interfaces;
using PriceRadar.Domain.RawEntities;
using PriceRadar.Parsers.Zoommer.JsonModels;

namespace PriceRadar.Parsers.Zoommer;

public class ZoommerParser : IParser
{
    private const string ProductUrl = "https://api.zoommer.ge/v1/Products/v3?CategoryId=";

    private readonly IStaticPageLoader _staticPageLoader;
    private readonly IStoreCategoryRepository _storeCategoryRepository;

    public ZoommerParser(IStaticPageLoader staticPageLoader, IStoreCategoryRepository storeCategoryRepository)
    {
        _staticPageLoader = staticPageLoader;
        _storeCategoryRepository = storeCategoryRepository;
    }

    public async Task<List<OfferRaw>> ParseAsync()
    {
        var categories =
            await _storeCategoryRepository.GetAllByStoreIdAsync(Constants.PredefinedIds.Stores.Zoommer);

        var offers = new HashSet<OfferRaw>();

        foreach (var category in categories)
        {
            var categoryUrl = Regex.Match(category.Url, @"c(\d+)").Groups[1].Value;
            var initialUrl = BuildPageUrl(categoryUrl, 1);

            var initialPage = await LoadPageAsync(initialUrl);
            if (initialPage == null)
            {
                continue;
            }

            offers.UnionWith(MapOffers(initialPage.Products, category.CategoryId));

            int totalPages = CalculateTotalPages(initialPage.TotalCount);
            if (totalPages <= 1)
            {
                continue;
            }

            for (int page = 2; page <= totalPages; page++)
            {
                var pageUrl = BuildPageUrl(categoryUrl, page);

                var pageData = await LoadPageAsync(pageUrl);
                if (pageData?.Products != null)
                {
                    offers.UnionWith(MapOffers(pageData.Products, category.CategoryId));
                }
            }
        }

        return offers.ToList();
    }

    private async Task<JsonProductList?> LoadPageAsync(string url)
    {
        var pageContent = await _staticPageLoader.LoadPageAsync(url, HttpMethod.Get, null);

        if (string.IsNullOrWhiteSpace(pageContent))
        {
            return null;
        }

        return JsonSerializer.Deserialize<JsonProductList>(pageContent);
    }

    private string BuildPageUrl(string categoryUrl, int page) => $"{ProductUrl}{categoryUrl}&Page={page}&Limit=28";

    private int CalculateTotalPages(int totalCount) => (int)Math.Ceiling(totalCount / 28.0);

    private IEnumerable<OfferRaw> MapOffers(List<JsonProduct> jsonProducts, Guid categoryId)
    {
        foreach (var jsonProduct in jsonProducts)
        {
            var hasDiscount = jsonProduct.PreviousPrice.HasValue &&
                              jsonProduct.PreviousPrice.Value > jsonProduct.ActualPrice;

            var offerRaw = new OfferRaw
            {
                Name = jsonProduct.Name,
                CategoryId = categoryId,
                Url = "https://zoommer.ge/en/" + jsonProduct.Route,
                Price = hasDiscount ? jsonProduct.PreviousPrice.Value : jsonProduct.ActualPrice,
                DiscountPrice = hasDiscount ? jsonProduct.ActualPrice : 0
            };

            yield return offerRaw;
        }
    }
}