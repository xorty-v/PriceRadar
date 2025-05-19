namespace PriceRadar.Application.Abstractions.Loaders;

public interface IBrowserPageLoader
{
    public Task<string> LoadPageAsync(string url, List<PageAction>? pageActions = null);
}