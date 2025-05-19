using AngleSharp;
using Microsoft.Extensions.DependencyInjection;
using PriceRadar.Application.Abstractions.Parsers;
using PriceRadar.Parsers.Alta;
using PriceRadar.Parsers.EliteElectronic;
using PriceRadar.Parsers.Zoommer;

namespace PriceRadar.Parsers;

public static class DependencyInjection
{
    public static IServiceCollection AddParsers(this IServiceCollection services)
    {
        services.AddSingleton<IBrowsingContext>(_ =>
        {
            var config = Configuration.Default;
            return BrowsingContext.New(config);
        });

        services.AddScoped<EliteElectronicParser>();
        services.AddScoped<ZoommerParser>();
        services.AddScoped<AltaParser>();

        services.AddTransient<IParserFactory, ParserFactory>();

        return services;
    }
}