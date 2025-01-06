using System.Net;
using System.Globalization;
using SolarWatch.Services.WebClientWrapper;

namespace SolarWatch.Services.ApiProviders
{
    public class SolarTimeProvider : ISolarTimeProvider
    {
        private readonly ILogger<SolarTimeProvider> _logger;
        private readonly IWebClient _client;
        public SolarTimeProvider(ILogger<SolarTimeProvider> logger, IWebClient client)
        {
            _logger = logger;
            _client = client;
        }

        public string GetSolarTimes(float lat, float lon, DateOnly date, string? tzid)
        {
            var formattedLat = lat.ToString(CultureInfo.InvariantCulture);
            var formattedLon = lon.ToString(CultureInfo.InvariantCulture);
            var formattedDate = date.ToString("yyyy-MM-dd");
            var url = $"https://api.sunrise-sunset.org/json?lat={formattedLat}&lng={formattedLon}&date={formattedDate}&formatted=0{(string.IsNullOrWhiteSpace(tzid) ? "" : $"&tzid={tzid}")}";
            _logger.LogDebug("Calling Sunrise/Sunset API with url: {url}", url);
            return _client.DownloadString(url);
        }
    }
}
