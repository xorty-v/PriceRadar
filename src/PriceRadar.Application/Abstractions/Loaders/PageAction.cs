using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace PriceRadar.Application.Abstractions.Loaders;

public record PageAction(PageActionType Type, params object[] Parameters);

[JsonConverter(typeof(StringEnumConverter))]
public enum PageActionType
{
    Click,
    Wait,
    ScrollToEnd
}