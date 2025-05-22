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
        var categoryUrls = categories.Select(c => c.Url.Split('/').Last()).ToList();

        for (int i = 0; i < categoryUrls.Count; i++)
        {
            var urlPart = categoryUrls[i];
            var content = PostRequestBody.Create(urlPart);

            var initialPage = await _staticPageLoader.LoadPageAsync(ProductUrl, HttpMethod.Post, content);
            if (string.IsNullOrWhiteSpace(initialPage)) continue;

            var response = JsonSerializer.Deserialize<JsonProductList>(initialPage);
            if (response?.Products == null) continue;

            offers.UnionWith(MapOffers(response.Products));

            int totalPages = (int)Math.Ceiling(response.TotalCount / 10.0);
            if (totalPages <= 1) continue;

            for (int page = 2; page <= totalPages; page++)
            {
                var additionalContent = PostRequestBody.Create(urlPart, page);
                var pageContent = await _staticPageLoader.LoadPageAsync(ProductUrl, HttpMethod.Post, additionalContent);

                if (string.IsNullOrWhiteSpace(pageContent)) continue;

                var additionalResponse = JsonSerializer.Deserialize<JsonProductList>(pageContent);
                if (additionalResponse?.Products != null)
                {
                    offers.UnionWith(MapOffers(response.Products));
                }
            }
        }

        return offers;
    }

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