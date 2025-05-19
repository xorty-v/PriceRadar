using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace PriceRadar.Application.Abstractions.Parsers;

[JsonConverter(typeof(StringEnumConverter))]
public enum StoreType
{
    EliteElectronic,
    Zoommer,
    Alta
}