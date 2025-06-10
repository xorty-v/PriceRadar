using Microsoft.Extensions.DependencyInjection;
using PriceRadar.Application.Abstractions;

namespace PriceRadar.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IParserService, ParserService>();
        services.AddScoped<IProductMatcherService, ProductMatcherService>();

        return services;
    }
}