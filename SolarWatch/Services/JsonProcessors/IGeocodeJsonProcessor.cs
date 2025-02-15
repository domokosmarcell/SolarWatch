using SolarWatch.Models;

namespace SolarWatch.Services.JsonProcessors
{
    public interface IGeocodeJsonProcessor
    {
        City ProcessGeocodeInfo(string geocodeInfo, string city);
    }
}
