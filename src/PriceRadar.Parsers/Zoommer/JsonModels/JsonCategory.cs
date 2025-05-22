using System.Text.Json.Serialization;

namespace PriceRadar.Parsers.Zoommer.JsonModels;

public class JsonCategory
{
    [JsonPropertyName("name")] 
    public string Name { get; set; }

    [JsonPropertyName("url")] 
    public string Url { get; set; }

    [JsonPropertyName("childCategories")]
    public List<JsonChildCategory> ChildCategories { get; set; }
}