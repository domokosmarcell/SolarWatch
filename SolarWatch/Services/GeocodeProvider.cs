using System.Net;

namespace SolarWatch.Services
{
    public class GeocodeProvider : IGeocodeProvider
    {
        private ILogger<GeocodeProvider> _logger;
        public GeocodeProvider(ILogger<GeocodeProvider> logger)
        {
            _logger = logger;
        }
        public string GetGeocode(string city)
        {
            var url = $"http://api.openweathermap.org/geo/1.0/direct?q={city}&limit=5&appid=Api-key";
            using var client = new WebClient();
            _logger.LogInformation($"Calling GeoCoding API with url: {url}");
            return client.DownloadString(url);
        }
    }
}
