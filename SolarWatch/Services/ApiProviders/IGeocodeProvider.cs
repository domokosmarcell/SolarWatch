namespace SolarWatch.Services.ApiProviders
{
    public interface IGeocodeProvider
    {
        string GetGeocode(string city);
    }
}
