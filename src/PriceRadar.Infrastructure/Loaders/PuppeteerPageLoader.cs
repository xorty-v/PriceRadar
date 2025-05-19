using PriceRadar.Application.Abstractions.Loaders;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.AnonymizeUa;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerSharp;

namespace PriceRadar.Infrastructure.Loaders;

internal sealed class PuppeteerPageLoader : BrowserPageLoader, IBrowserPageLoader
{
    private IBrowser _browser;

    public async Task<string> LoadPageAsync(string url, List<PageAction>? pageActions = null)
    {
        if (_browser == null)
        {
            _browser = await InitializeBrowserAsync();
        }

        await using var page = await _browser.NewPageAsync();
        await ConfigurePageAsync(page);

        await page.GoToAsync(url, WaitUntilNavigation.DOMContentLoaded);

        if (pageActions != null)
            foreach (var action in pageActions)
                await PageActions[action.Type](page, action.Parameters);

        var html = await page.GetContentAsync();

        return html;
    }

    private async Task<IBrowser> InitializeBrowserAsync()
    {
        var puppeteerExtra = new PuppeteerExtra().Use(new AnonymizeUaPlugin()).Use(new StealthPlugin());

        return await puppeteerExtra.LaunchAsync(ConfigureLaunchAsync());
    }

    #region Configuring Browser Settings

    private LaunchOptions ConfigureLaunchAsync()
    {
        return new LaunchOptions
        {
            Headless = false,
            Args = new[]
            {
                "--disable-blink-features=AutomationControlled",
                "--disable-infobars",
                "--disable-extensions",
                "--no-first-run",
                "--no-sandbox",
                "--disable-setuid-sandbox",
                "--disable-dev-shm-usage",
                "--no-zygote"
            },
            IgnoredDefaultArgs = new[] { "enable-automation" }
        };
    }

    private async Task ConfigurePageAsync(IPage page)
    {
        await page.SetViewportAsync(new ViewPortOptions { Width = 1280, Height = 800 });

        await page.SetRequestInterceptionAsync(true);
        page.Request += (_, e) =>
        {
            if (e.Request.ResourceType == ResourceType.Image ||
                e.Request.ResourceType == ResourceType.StyleSheet ||
                e.Request.ResourceType == ResourceType.Media ||
                e.Request.ResourceType == ResourceType.Font)
            {
                e.Request.AbortAsync();
            }
            else
            {
                e.Request.ContinueAsync();
            }
        };
    }

    #endregion
}