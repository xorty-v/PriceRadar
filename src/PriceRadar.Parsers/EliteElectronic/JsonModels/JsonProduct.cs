using System.Text.Json.Serialization;

namespace PriceRadar.Parsers.EliteElectronic.JsonModels;

public class JsonProduct
{
    [JsonPropertyName("product_desc")] public string Name { get; set; }

    [JsonPropertyName("actual_price")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal ActualPrice { get; set; }

    [JsonPropertyName("sale_price")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal SalePrice { get; set; }

    [JsonPropertyName("product_slug")] public string ProductSlug { get; set; }

    [JsonPropertyName("category_slug")] public string Category { get; set; }

    [JsonPropertyName("parent_category_slug")]
    public string ParentCategory { get; set; }
}