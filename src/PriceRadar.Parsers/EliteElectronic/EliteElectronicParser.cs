using System.Text.Json;
using AngleSharp;
using PriceRadar.Application;
using PriceRadar.Application.Abstractions.Loaders;
using PriceRadar.Application.Abstractions.Parsers;
using PriceRadar.Domain.RawEntities;
using PriceRadar.Parsers.Abstractions;
using PriceRadar.Parsers.EliteElectronic.JsonModels;
using PriceRadar.Parsers.EliteElectronic.Requests;

namespace PriceRadar.Parsers.EliteElectronic;

public class EliteElectronicParser : BaseParser, IParser
{
    private readonly IBrowsingContext _browsingContext;
    private readonly ICategoryMapperService _categoryMapperService;
    private readonly IStaticPageLoader _staticPageLoader;

    public EliteElectronicParser(
        IStaticPageLoader staticPageLoader,
        IBrowsingContext browsingContext,
        ICategoryMapperService categoryMapperService)
    {
        _staticPageLoader = staticPageLoader;
        _browsingContext = browsingContext;
        _categoryMapperService = categoryMapperService;
    }

    protected override string CategoryUrl { get; } = "https://ee.ge/";
    protected override string ProductUrl { get; } = "https://api.ee.ge/product/filter_products";

    public async Task<List<OfferRaw>> ParseAsync()
    {
        var categories = await ParseCategoriesAsync();
        var offers = await ParseOffersAsync(categories);

        return offers.ToList();
    }

    public override async Task<List<CategoryRaw>> ParseCategoriesAsync()
    {
        var pageContent = await _staticPageLoader.LoadPageAsync(CategoryUrl, HttpMethod.Get, null);
        var document = await _browsingContext.OpenAsync(req => req.Content(pageContent));

        var categories = document
            .QuerySelectorAll("div.dropdown-content a")
            .Select(a => new CategoryRaw
            {
                Name = a.TextContent.Trim(),
                Url = a.GetAttribute("href")
            })
            .Where(c => _categoryMapperService.Map(StoreType.EliteElectronic, c.Name) != null)
            .ToList();

        return categories;
    }

    public override async Task<HashSet<OfferRaw>> ParseOffersAsync(List<CategoryRaw> categories)
    {
        var offers = new HashSet<OfferRaw>();
        var categoryUrls = ExtractCategoryUrls(categories);

        foreach (var categoryUrl in categoryUrls)
        {
            var initialPage = await LoadProductPageAsync(categoryUrl, 1);
            if (initialPage?.Products == null)
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
                var additionalData = await LoadProductPageAsync(categoryUrl, page);
                if (additionalData?.Products != null)
                {
                    offers.UnionWith(MapOffers(additionalData.Products));
                }
            }
        }

        return offers;
    }

    private List<string> ExtractCategoryUrls(List<CategoryRaw> categories)
    {
        return categories
            .Select(c => c.Url.Split('/').Last())
            .Where(part => !string.IsNullOrEmpty(part))
            .ToList();
    }

    private async Task<JsonProductList?> LoadProductPageAsync(string categoryUrl, int page)
    {
        var requestBody = PostRequestBody.Create(categoryUrl, page);
        var pageContent = await _staticPageLoader.LoadPageAsync(ProductUrl, HttpMethod.Post, requestBody);

        if (string.IsNullOrWhiteSpace(pageContent))
        {
            return null;
        }

        return JsonSerializer.Deserialize<JsonProductList>(pageContent);
    }

    private int CalculateTotalPages(int totalCount) => (int)Math.Ceiling(totalCount / 10.0);

    private IEnumerable<OfferRaw> MapOffers(List<JsonProduct> jsonProducts)
    {
        foreach (var jsonProduct in jsonProducts)
        {
            var offerRaw = new OfferRaw
            {
                Name = jsonProduct.Name,
                Category = jsonProduct.Category,
                Url = $"https://ee.ge/{jsonProduct.ParentCategory}/{jsonProduct.Category}/{jsonProduct.ProductSlug}",
                Price = jsonProduct.ActualPrice,
                DiscountPrice = jsonProduct.SalePrice
            };

            yield return offerRaw;
        }
    }
}