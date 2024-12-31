namespace SolarWatch.Services.JsonProcessors
{
    public interface IGeocodeJsonProcessor
    {
        (double, double) ProcessGeocodeInfo(string geocodeInfo);
    }
}
