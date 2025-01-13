namespace SolarWatch.Services.ApiProviders
{
    public interface IGeocodeProvider
    {
        Task<string> GetGeocode(string city);
    }
}
