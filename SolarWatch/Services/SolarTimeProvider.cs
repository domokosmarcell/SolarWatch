using System.Net;
using System.Globalization;

namespace SolarWatch.Services
{
    public class SolarTimeProvider : ISolarTimeProvider
    {
        private ILogger<SolarTimeProvider> _logger;
        public SolarTimeProvider(ILogger<SolarTimeProvider> logger)
        {
            _logger = logger;
        }
        
        public string GetSolarTimes(float lat, float lon, DateOnly date, string? tzid)
        {
            var formattedLat = lat.ToString(CultureInfo.InvariantCulture);
            var formattedLon = lon.ToString(CultureInfo.InvariantCulture);
            var formattedDate = date.ToString("yyyy-MM-dd");
            var url = $"https://api.sunrise-sunset.org/json?lat={formattedLat}&lng={formattedLon}&date={formattedDate}&formatted=0{(string.IsNullOrWhiteSpace(tzid) ? "" : $"&tzid={tzid}")}";
            using var client = new WebClient();
            _logger.LogDebug("Calling Sunrise/Sunset API with url: {url}", url);
            return client.DownloadString(url);
        }
    }
}
