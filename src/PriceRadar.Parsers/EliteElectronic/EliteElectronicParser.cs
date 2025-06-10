using System.Text.Json;
using PriceRadar.Application.Abstractions.Loaders;
using PriceRadar.Application.Abstractions.Parsers;
using PriceRadar.Domain;
using PriceRadar.Domain.Interfaces;
using PriceRadar.Domain.RawEntities;
using PriceRadar.Parsers.EliteElectronic.JsonModels;
using PriceRadar.Parsers.EliteElectronic.Requests;

namespace PriceRadar.Parsers.EliteElectronic;

public class EliteElectronicParser : IParser
{
    private const string ProductUrl = "https://api.ee.ge/product/filter_products";

    private readonly IStaticPageLoader _staticPageLoader;
    private readonly IStoreCategoryRepository _storeCategoryRepository;

    public EliteElectronicParser(IStaticPageLoader staticPageLoader, IStoreCategoryRepository storeCategoryRepository)
    {
        _staticPageLoader = staticPageLoader;
        _storeCategoryRepository = storeCategoryRepository;
    }

    public async Task<List<OfferRaw>> ParseAsync()
    {
        var categories =
            await _storeCategoryRepository.GetCategoriesByStoreAsync(Constants.PredefinedIds.Stores.EliteElectronic);

        var offers = new HashSet<OfferRaw>();

        foreach (var category in categories)
        {
            var categoryUrl = category.Url.Split('/').Last();
            var initialPage = await LoadPageAsync(categoryUrl, 1);
            if (initialPage?.Products == null)
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
                var additionalData = await LoadPageAsync(categoryUrl, page);
                if (additionalData?.Products != null)
                {
                    offers.UnionWith(MapOffers(additionalData.Products, category.CategoryId));
                }
            }
        }

        return offers.ToList();
    }

    private async Task<JsonProductList?> LoadPageAsync(string categoryUrl, int page)
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

    private IEnumerable<OfferRaw> MapOffers(List<JsonProduct> jsonProducts, Guid categoryId)
    {
        foreach (var jsonProduct in jsonProducts)
        {
            var offerRaw = new OfferRaw
            {
                Name = jsonProduct.Name,
                CategoryId = categoryId,
                Url = $"https://ee.ge/{jsonProduct.ParentCategory}/{jsonProduct.Category}/{jsonProduct.ProductSlug}",
                Price = jsonProduct.ActualPrice,
                DiscountPrice = jsonProduct.SalePrice
            };

            yield return offerRaw;
        }
    }
}