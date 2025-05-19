using AngleSharp;
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
    protected override string ProductUrl { get; }

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
            var pageIndex = 1;

            while (true)
            {
                var pageUrl = pageIndex == 1
                    ? category.Url
                    : category.Url.Replace(".html", $"-page-{pageIndex}.html");

                var pageContent = await _browserPageLoader.LoadPageAsync(pageUrl);
                var document = await _browsingContext.OpenAsync(req => req.Content(pageContent));

                var productBlocks = document.QuerySelectorAll("div.ty-grid-list__item");
                if (productBlocks.Length == 0)
                    break;

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

                        var discount = ParsePrice(priceText);
                        var oldPrice = ParsePrice(oldPriceText);

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
                            Category = category.Name
                        });
                    }
                }

                pageIndex++;
            }
        }

        return offers;
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