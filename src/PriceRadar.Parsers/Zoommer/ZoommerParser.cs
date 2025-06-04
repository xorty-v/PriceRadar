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
        var categoryUrls = ExtractCategoryUrls(categories);

        foreach (var categoryUrl in categoryUrls)
        {
            var initialUrl = BuildPageUrl(categoryUrl, 1);

            var initialPage = await LoadPageAsync(initialUrl);
            if (initialPage == null)
            {
                continue;
            }

            offers.UnionWith(MapOffers(initialPage.Products));

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
                    offers.UnionWith(MapOffers(pageData.Products));
                }
            }
        }

        return offers;
    }

    private List<string> ExtractCategoryUrls(List<CategoryRaw> categories)
    {
        return categories
            .Select(c => Regex.Match(c.Url, @"\d+$").Value)
            .Where(url => !string.IsNullOrEmpty(url))
            .ToList();
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