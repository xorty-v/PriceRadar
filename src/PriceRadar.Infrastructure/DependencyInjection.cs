using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PriceRadar.Application.Abstractions.Loaders;
using PriceRadar.Domain.Interfaces;
using PriceRadar.Infrastructure.Loaders;
using PriceRadar.Infrastructure.Repositories;

namespace PriceRadar.Infrastructure;

public sealed class HttpClientNames
{
    public const string PageLoader = "PageLoader";
}

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddLoaders(services);
        AddPersistence(services, configuration);

        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database") ??
                               throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<ApplicationDbContext>(options => { options.UseNpgsql(connectionString); });

        services.AddScoped<IStoreRepository, StoreRepository>();
        services.AddScoped<IOfferRepository, OfferRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IStoreCategoryRepository, StoreCategoryRepository>();
    }

    private static void AddLoaders(IServiceCollection services)
    {
        services.AddSingleton<IBrowserPageLoader, PuppeteerPageLoader>();
        services.AddSingleton<IStaticPageLoader, HttpStaticPageLoader>();

        services.AddHttpClient(HttpClientNames.PageLoader, client => { client.Timeout = TimeSpan.FromSeconds(5); })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var cookieContainer = new CookieContainer();
                cookieContainer.Add(new Uri("https://ee.ge/"), new Cookie("lang", "en"));

                return new HttpClientHandler
                {
                    UseCookies = true,
                    CookieContainer = cookieContainer
                };
            });
    }
}