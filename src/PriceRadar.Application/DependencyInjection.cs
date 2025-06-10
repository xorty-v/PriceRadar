using Microsoft.Extensions.DependencyInjection;
using PriceRadar.Application.Abstractions;

namespace PriceRadar.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IParserService, ParserService>();
        services.AddScoped<IProductMatcherService, ProductMatcherService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<IStoreCategoryService, StoreCategoryService>();

        return services;
    }
}