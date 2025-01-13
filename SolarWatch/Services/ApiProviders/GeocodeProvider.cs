using SolarWatch.Services.HttpClientWrapper;

namespace SolarWatch.Services.ApiProviders
{
    public class GeocodeProvider : IGeocodeProvider
    {
        private readonly ILogger<GeocodeProvider> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClient _client;

        public GeocodeProvider(ILogger<GeocodeProvider> logger, IConfiguration configuration, IHttpClient client)
        {
            _logger = logger;
            _configuration = configuration;
            _client = client;
        }
        public async Task<string> GetGeocode(string city)
        {
            var url = $"http://api.openweathermap.org/geo/1.0/direct?q={city}&limit=1&appid={_configuration["ApiKeys:OpenWeatherMap"]}";
            _logger.LogDebug("Calling GeoCoding API with url: {url}", url);
            return await _client.GetStringAsync(url);
        }
    }
}
