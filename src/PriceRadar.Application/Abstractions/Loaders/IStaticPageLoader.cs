namespace PriceRadar.Application.Abstractions.Loaders;

public interface IStaticPageLoader
{
    public Task<string> LoadPageAsync(string url, HttpMethod method, HttpContent? content);
}