using System.Text.Json.Serialization;

namespace PriceRadar.Parsers.Zoommer.JsonModels;

public class JsonProductList
{
    [JsonPropertyName("productsCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("products")]
    public List<JsonProduct>? Products { get; set; }
}