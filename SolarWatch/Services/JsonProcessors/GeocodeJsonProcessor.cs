
using System.Text.Json;

namespace SolarWatch.Services.JsonProcessors
{
    public class GeocodeJsonProcessor : IGeocodeJsonProcessor
    {
        public (float, float) ProcessGeocodeInfo(string geocodeInfo)
        {
            JsonDocument json = JsonDocument.Parse(geocodeInfo);
            var geocodeInfoElement = json.RootElement[0];
            //handle the case when the array is empty
            float lat = geocodeInfoElement.GetProperty("lat").GetSingle();
            float lon = geocodeInfoElement.GetProperty("lon").GetSingle();

            return (lat, lon);
        }
    }
}
