using System.Text.Json;
using System.Text.RegularExpressions;
using PriceRadar.Application;
using PriceRadar.Application.Abstractions.Loaders;
using PriceRadar.Application.Abstractions.Parsers;
using PriceRadar.Domain.RawEntities;
using PriceRadar.Parsers.Abstractions;
using PriceRadar.Parsers.Zoommer.Helpers;
using PriceRadar.Parsers.Zoommer.JsonModels;

namespace PriceRadar.Parsers.Zoommer;

public class ZoommerParser : BaseParser, IParser
{
    private readonly ICategoryMapperService _categoryMapperService;
    private readonly IStaticPageLoader _staticPageLoader;

    public ZoommerParser(IStaticPageLoader staticPageLoader, ICategoryMapperService categoryMapperService)
    {
        _staticPageLoader = staticPageLoader;
        _categoryMapperService = categoryMapperService;
    }

    protected override string CategoryUrl { get; } = "https://api.zoommer.ge/v1/Categories/all-categories";
    protected override string ProductUrl { get; } = "https://api.zoommer.ge/v1/Products/v3?CategoryId=";

    public async Task<List<OfferRaw>> ParseAsync()
    {
        var categories = await ParseCategoriesAsync();
        var offers = await ParseOffersAsync(categories);

        return offers.ToList();
    }

    public override async Task<List<CategoryRaw>> ParseCategoriesAsync()
    {
        var page = await _staticPageLoader.LoadPageAsync(CategoryUrl, HttpMethod.Get, null);
        var response = JsonSerializer.Deserialize<List<JsonCategory>>(page);

        var categories = response
            .SelectMany(category => new[]
            {
                new CategoryRaw { Name = category.Name, Url = category.Url }
            }.Concat(category.ChildCategories.Select(child => new CategoryRaw
            {
                Name = child.Name,
                Url = child.Url
            })))
            .Where(c => _categoryMapperService.Map(StoreType.Zoommer, c.Name) != null)
            .DistinctBy(c => c.Name)
            .ToList();

        return categories;
    }

    public override async Task<HashSet<OfferRaw>> ParseOffersAsync(List<CategoryRaw> categories)
    {
        var offers = new HashSet<OfferRaw>();
        var categoryUrls = categories.Select(c => Regex.Match(c.Url, @"\d+$").Value).ToList();

        for (int i = 0; i < categoryUrls.Count; i++)
        {
            string categoryUrl = categoryUrls[i];
            string initialUrl = $"{ProductUrl}{categoryUrl}&Page=1&Limit=28";

            string initialPage = await _staticPageLoader.LoadPageAsync(initialUrl, HttpMethod.Get, null);
            if (string.IsNullOrWhiteSpace(initialPage)) continue;

            var response = JsonSerializer.Deserialize<JsonProductList>(initialPage);
            if (response?.Products == null) continue;

            offers.UnionWith(MapOffers(response.Products));

            int totalPages = (int)Math.Ceiling(response.TotalCount / 28.0);
            if (totalPages <= 1) continue;

            for (int page = 2; page <= totalPages; page++)
            {
                string pageUrl = $"{ProductUrl}{categoryUrl}&Page={page}&Limit=28";
                string pageContent = await _staticPageLoader.LoadPageAsync(pageUrl, HttpMethod.Get, null);

                if (string.IsNullOrWhiteSpace(pageContent)) continue;

                var additionalResponse = JsonSerializer.Deserialize<JsonProductList>(pageContent);
                if (additionalResponse?.Products != null)
                {
                    offers.UnionWith(MapOffers(additionalResponse.Products));
                }
            }
        }

        return offers;
    }

    private IEnumerable<OfferRaw> MapOffers(List<JsonProduct> jsonProducts)
    {
        foreach (var jsonProduct in jsonProducts)
        {
            var hasDiscount = jsonProduct.PreviousPrice.HasValue &&
                              jsonProduct.PreviousPrice.Value > jsonProduct.ActualPrice;

            var offerRaw = new OfferRaw
            {
                Name = jsonProduct.Name,
                Category = CategoryMap.GetCategoryName(jsonProduct.CategoryId, jsonProduct.SubCategoryId),
                Url = "https://zoommer.ge/en/" + jsonProduct.Route,
                Price = hasDiscount ? jsonProduct.PreviousPrice.Value : jsonProduct.ActualPrice,
                DiscountPrice = hasDiscount ? jsonProduct.ActualPrice : 0
            };

            yield return offerRaw;
        }
    }
}