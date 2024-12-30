namespace SolarWatch.Services
{
    public interface IGeocodeJsonProcessor
    {
        (double, double) ProcessGeocodeInfo(string geocodeInfo);
    }
}
