using AngleSharp;
using AngleSharp.Dom;
using PriceRadar.Application;
using PriceRadar.Application.Abstractions.Loaders;
using PriceRadar.Application.Abstractions.Parsers;
using PriceRadar.Domain.RawEntities;
using PriceRadar.Parsers.Abstractions;

namespace PriceRadar.Parsers.Alta;

public class AltaParser : BaseParser, IParser
{
    private readonly IBrowserPageLoader _browserPageLoader;
    private readonly IBrowsingContext _browsingContext;
    private readonly ICategoryMapperService _categoryMapperService;

    public AltaParser(
        IBrowserPageLoader browserPageLoader,
        IBrowsingContext browsingContext,
        ICategoryMapperService categoryMapperService)
    {
        _browserPageLoader = browserPageLoader;
        _browsingContext = browsingContext;
        _categoryMapperService = categoryMapperService;
    }

    protected override string CategoryUrl { get; } = "https://alta.ge/?sl=en";
    protected override string ProductUrl { get; } = string.Empty;

    public async Task<List<OfferRaw>> ParseAsync()
    {
        var categories = await ParseCategoriesAsync();
        var offers = await ParseOffersAsync(categories);

        return offers.ToList();
    }

    public override async Task<List<CategoryRaw>> ParseCategoriesAsync()
    {
        var pageContent = await _browserPageLoader.LoadPageAsync(CategoryUrl);
        var document = await _browsingContext.OpenAsync(req => req.Content(pageContent));

        var categories = document
            .QuerySelectorAll("li.ty-menu__submenu-item > a.ty-menu__submenu-link")
            .Concat(document.QuerySelectorAll("div.ty-menu__submenu-item-header > a.ty-menu__submenu-link"))
            .Select(a => new CategoryRaw
            {
                Name = a.TextContent.Trim(),
                Url = a.GetAttribute("href")?.Trim()
            })
            .Where(c => _categoryMapperService.Map(StoreType.Alta, c.Name) != null)
            .ToList();

        return categories;
    }

    public override async Task<HashSet<OfferRaw>> ParseOffersAsync(List<CategoryRaw> categories)
    {
        var offers = new HashSet<OfferRaw>();

        foreach (var category in categories)
        {
            var firstPageContent = await _browserPageLoader.LoadPageAsync(category.Url);
            if (string.IsNullOrWhiteSpace(firstPageContent))
            {
                continue;
            }

            var firstDocument = await _browsingContext.OpenAsync(req => req.Content(firstPageContent));
            ExtractOffersFromPage(firstDocument, category.Name, offers);

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
                ExtractOffersFromPage(document, category.Name, offers);
            }
        }

        return offers;
    }

    private void ExtractOffersFromPage(IDocument document, string categoryName, HashSet<OfferRaw> offers)
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
                    Category = categoryName
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

    private async Task<List<string>> LoadPagesAsync1(List<string> urls)
    {
        var loadTasks = urls.Select(url => _browserPageLoader.LoadPageAsync(url));
        return (await Task.WhenAll(loadTasks)).ToList();
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