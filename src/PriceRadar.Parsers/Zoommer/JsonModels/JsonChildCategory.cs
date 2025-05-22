using System.Text.Json.Serialization;

namespace PriceRadar.Parsers.Zoommer.JsonModels;

public class JsonChildCategory
{
    [JsonPropertyName("name")] 
    public string Name { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}