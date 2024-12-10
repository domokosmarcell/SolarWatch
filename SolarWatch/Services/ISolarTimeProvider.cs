namespace SolarWatch.Services
{
    public interface ISolarTimeProvider
    {
        string GetSolarTimes(double lat, double lon, DateOnly date);
    }
}
