using System.Net;

namespace SolarWatch.Services
{
    public class GeocodeProvider : IGeocodeProvider
    {
        private readonly ILogger<GeocodeProvider> _logger;
        private readonly IConfiguration _configuration;
        public GeocodeProvider(ILogger<GeocodeProvider> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public string GetGeocode(string city)
        {

            var url = $"http://api.openweathermap.org/geo/1.0/direct?q={city}&limit=1&appid={_configuration["ApiKeys:OpenWeatherMap"]}";
            using var client = new WebClient();
            _logger.LogDebug("Calling GeoCoding API with url: {url}", url);
            return client.DownloadString(url);
        }
    }
}
