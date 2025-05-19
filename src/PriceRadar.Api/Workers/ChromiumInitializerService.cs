using PuppeteerSharp;

namespace PriceRadar.Api.Workers;

internal sealed class ChromiumInitializerService : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await new BrowserFetcher().DownloadAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}