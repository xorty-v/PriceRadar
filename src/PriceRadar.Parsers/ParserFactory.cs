using Microsoft.Extensions.DependencyInjection;
using PriceRadar.Application.Abstractions.Parsers;
using PriceRadar.Parsers.Alta;
using PriceRadar.Parsers.EliteElectronic;
using PriceRadar.Parsers.Zoommer;

namespace PriceRadar.Parsers;

internal sealed class ParserFactory : IParserFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ParserFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IParser CreateParser(StoreType storeType)
    {
        return storeType switch
        {
            StoreType.EliteElectronic => _serviceProvider.GetService<EliteElectronicParser>(),
            StoreType.Zoommer => _serviceProvider.GetService<ZoommerParser>(),
            StoreType.Alta => _serviceProvider.GetService<AltaParser>(),
            _ => throw new ArgumentException("Unknown store type")
        };
    }
}