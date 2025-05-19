using PriceRadar.Application.Abstractions.Loaders;

namespace PriceRadar.Infrastructure.Loaders;

internal sealed class HttpStaticPageLoader : IStaticPageLoader
{
    private readonly HttpClient _httpClient;

    public HttpStaticPageLoader(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientNames.PageLoader);
    }

    public async Task<string> LoadPageAsync(string url, HttpMethod method, HttpContent? content)
    {
        var request = new HttpRequestMessage(method, url)
        {
            Content = content
        };

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Failed to load page {url}. Error code: {response.StatusCode}");
        }

        return await response.Content.ReadAsStringAsync();
    }
}