using AngleSharp;
using AngleSharp.Dom;
using PriceRadar.Application.Abstractions.Loaders;
using PriceRadar.Application.Abstractions.Parsers;
using PriceRadar.Domain;
using PriceRadar.Domain.Entities;
using PriceRadar.Domain.Interfaces;
using PriceRadar.Domain.RawEntities;

namespace PriceRadar.Parsers.Alta;

public class AltaParser : IParser
{
    private readonly IBrowserPageLoader _browserPageLoader;
    private readonly IBrowsingContext _browsingContext;
    private readonly IStoreCategoryRepository _storeCategoryRepository;

    public AltaParser(
        IBrowserPageLoader browserPageLoader,
        IBrowsingContext browsingContext,
        IStoreCategoryRepository storeCategoryRepository)
    {
        _browserPageLoader = browserPageLoader;
        _browsingContext = browsingContext;
        _storeCategoryRepository = storeCategoryRepository;
    }

    public async Task<List<OfferRaw>> ParseAsync()
    {
        var categories = await _storeCategoryRepository.GetCategoriesByStoreAsync(Constants.PredefinedIds.Stores.Alta);

        var offers = new HashSet<OfferRaw>();

        foreach (var category in categories)
        {
            var firstPageContent = await _browserPageLoader.LoadPageAsync(category.Url);
            if (string.IsNullOrWhiteSpace(firstPageContent))
            {
                continue;
            }

            var firstDocument = await _browsingContext.OpenAsync(req => req.Content(firstPageContent));
            ExtractOffersFromPage(firstDocument, category, offers);

            int totalPages = CalculateTotalPages(firstDocument);
            if (totalPages <= 1)
            {
                continue;
            }

            var additionalUrls = BuildPageUrls(category.Url, totalPages);

            var loadTasks = additionalUrls.Select(url => _browserPageLoader.LoadPageAsync(url));
            var pageContents = await Task.WhenAll(loadTasks);

            foreach (var content in pageContents.Where(c => !string.IsNullOrWhiteSpace(c)))
            {
                var document = await _browsingContext.OpenAsync(req => req.Content(content));
                ExtractOffersFromPage(document, category, offers);
            }
        }

        return offers.ToList();
    }

    private void ExtractOffersFromPage(IDocument document, StoreCategory category, HashSet<OfferRaw> offers)
    {
        var productBlocks = document.QuerySelectorAll("div.ty-grid-list__item");

        foreach (var block in productBlocks)
        {
            var titleElement = block.QuerySelector("a.product-title");
            var name = titleElement?.TextContent.Trim();
            var url = titleElement?.GetAttribute("href")?.Trim();

            var priceText = block.QuerySelector("span.ty-price-num")?.TextContent.Trim();
            var oldPriceText = block.QuerySelector("span.ty-list-price .ty-strike span")?.TextContent.Trim();

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(url))
            {
                var fullUrl = url.StartsWith("http") ? url : $"https://alta.ge{url}";
                decimal? discount = ParsePrice(priceText);
                decimal? oldPrice = ParsePrice(oldPriceText);

                decimal price, discountPrice;
                if (oldPrice.HasValue && oldPrice.Value > 0)
                {
                    price = oldPrice.Value;
                    discountPrice = discount ?? 0;
                }
                else
                {
                    price = discount ?? 0;
                    discountPrice = 0;
                }

                offers.Add(new OfferRaw
                {
                    Name = name,
                    Url = fullUrl,
                    Price = price,
                    DiscountPrice = discountPrice,
                    CategoryId = category.CategoryId
                });
            }
        }
    }

    private int CalculateTotalPages(IDocument document)
    {
        var countText = document.QuerySelector("div.count-cp")?.TextContent?.Trim().Split()[0];
        if (int.TryParse(countText, out int totalCount))
        {
            return Math.Max(1, (int)Math.Ceiling(totalCount / 16.0));
        }

        return 1;
    }

    private List<string> BuildPageUrls(string baseUrl, int totalPages)
    {
        var urls = new List<string>();
        for (int i = 2; i <= totalPages; i++)
        {
            urls.Add(baseUrl.Replace(".html", $"-page-{i}.html"));
        }

        return urls;
    }

    private decimal? ParsePrice(string? priceText)
    {
        if (string.IsNullOrWhiteSpace(priceText))
        {
            return null;
        }

        var digits = new string(priceText.Where(char.IsDigit).ToArray());

        return decimal.TryParse(digits, out var price) ? price : null;
    }
}