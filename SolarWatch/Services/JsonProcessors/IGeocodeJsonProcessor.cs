namespace SolarWatch.Services.JsonProcessors
{
    public interface IGeocodeJsonProcessor
    {
        (float, float) ProcessGeocodeInfo(string geocodeInfo, string city);
    }
}
