namespace SolarWatch.Services
{
    public interface ISolarTimeProvider
    {
        string GetSolarTimes(float lat, float lon, DateOnly date, string tzid);
    }
}
