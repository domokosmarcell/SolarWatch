namespace SolarWatch.Services.ApiProviders
{
    public interface ISolarTimeProvider
    {
        Task<string> GetSolarTimes(float lat, float lon, DateOnly date, string tzid);
    }
}
