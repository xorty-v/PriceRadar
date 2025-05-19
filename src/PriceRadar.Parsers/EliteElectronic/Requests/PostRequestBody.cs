using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PriceRadar.Parsers.EliteElectronic.Requests;

public class PostRequestBody
{
    [JsonPropertyName("min_price")] public int MinPrice { get; set; }

    [JsonPropertyName("max_price")] public int MaxPrice { get; set; }

    [JsonPropertyName("category")] public string Category { get; set; }

    [JsonPropertyName("brand")] public string[] Brand { get; set; }

    [JsonPropertyName("color")] public string[] Color { get; set; }

    [JsonPropertyName("room")] public string[] Room { get; set; }

    [JsonPropertyName("sort_by")] public string SortBy { get; set; }

    [JsonPropertyName("item_per_page")] public int ItemPerPage { get; set; } = 10;

    [JsonPropertyName("page_no")] public int PageNo { get; set; } = 1;

    [JsonPropertyName("specification")] public string[] Specification { get; set; }

    [JsonPropertyName("sale_products")] public int SaleProducts { get; set; }

    [JsonPropertyName("search_text")] public string SearchText { get; set; }

    [JsonPropertyName("slug")] public string Slug { get; set; }

    [JsonPropertyName("pageno")] public int? PageNoAlt { get; set; }

    public static HttpContent Create(string category, int page = 1, int itemsPerPage = 10)
    {
        var body = new PostRequestBody
        {
            Category = category,
            PageNo = page,
            ItemPerPage = itemsPerPage
        };

        var json = JsonSerializer.Serialize(body);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}