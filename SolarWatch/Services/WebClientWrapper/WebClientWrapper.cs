using System.Net;

namespace SolarWatch.Services.WebClientWrapper
{
    public class WebClientWrapper : IWebClient
    {
        public string DownloadString(string url)
        {
            using var client = new WebClient();
            return client.DownloadString(url);
        }
    }
}
