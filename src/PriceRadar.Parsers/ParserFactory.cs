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

    public IParser CreateParser(string storeName)
    {
        return storeName switch
        {
            "EliteElectronic" => _serviceProvider.GetService<EliteElectronicParser>(),
            "Zoommer" => _serviceProvider.GetService<ZoommerParser>(),
            "Alta" => _serviceProvider.GetService<AltaParser>(),
            _ => throw new ArgumentException("Unknown store name")
        };
    }
}