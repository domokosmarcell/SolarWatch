
using System.Text.Json;

namespace SolarWatch.Services
{
    public class GeocodeJsonProcessor : IGeocodeJsonProcessor
    {
        public (double, double) ProcessGeocodeInfo(string geocodeInfo)
        {
            JsonDocument json = JsonDocument.Parse(geocodeInfo);
            var geocodeInfoElement = json.RootElement[0];
            double lat = geocodeInfoElement.GetProperty("lat").GetDouble();
            double lon = geocodeInfoElement.GetProperty("lon").GetDouble();

            return (lat, lon);
        }
    }
}
