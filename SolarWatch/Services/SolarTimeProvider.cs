using System.Net;

namespace SolarWatch.Services
{
    public class SolarTimeProvider : ISolarTimeProvider
    {
        private ILogger<SolarTimeProvider> _logger;
        public SolarTimeProvider(ILogger<SolarTimeProvider> logger)
        {
            _logger = logger;
        }
        
        public string GetSolarTimes(double lat, double lon, DateOnly date, string tzid)
        {
            var url = $"https://api.sunrise-sunset.org/json?lat={lat}&lng={lon}&date={date}&tzid={tzid}";
            using var client = new WebClient();
            _logger.LogDebug("Calling Sunrise/Sunset API with url: {url}", url);
            return client.DownloadString(url);
        }
    }
}
