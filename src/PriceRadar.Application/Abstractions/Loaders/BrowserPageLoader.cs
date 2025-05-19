using PuppeteerSharp;

namespace PriceRadar.Application.Abstractions.Loaders;

public abstract class BrowserPageLoader
{
    protected readonly Dictionary<PageActionType, Func<IPage, object[], Task>> PageActions = new()
    {
        { PageActionType.Wait, async (_, data) => await Task.Delay(Convert.ToInt32(data.First())) },
        { PageActionType.Click, async (page, data) => await page.ClickAsync((string)data.First()) },
        {
            PageActionType.ScrollToEnd,
            async (page, _) => await page
                .EvaluateExpressionAsync("window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' });")
        }
    };
}