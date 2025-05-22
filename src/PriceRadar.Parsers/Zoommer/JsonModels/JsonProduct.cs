using System.Text.Json.Serialization;

namespace PriceRadar.Parsers.Zoommer.JsonModels;

public class JsonProduct
{
    [JsonPropertyName("categoryId")] 
    public int CategoryId { get; set; }

    [JsonPropertyName("subCategoryId")] 
    public int SubCategoryId { get; set; }

    [JsonPropertyName("name")] 
    public string Name { get; set; }

    [JsonPropertyName("price")] 
    public decimal ActualPrice { get; set; }

    [JsonPropertyName("previousPrice")] 
    public decimal? PreviousPrice { get; set; }

    [JsonPropertyName("route")] 
    public string Route { get; set; }

    [JsonPropertyName("parentCategoryName")]
    public string ParentCategory { get; set; }
}