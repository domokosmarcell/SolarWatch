using System.Net;

namespace SolarWatch.Services.HttpClientWrapper
{
    public class HttpClientWrapper : IHttpClient
    {
        public async Task<string> GetStringAsync(string url)
        {
            using var client = new HttpClient();
            return await client.GetStringAsync(url);
        }
    }
}
