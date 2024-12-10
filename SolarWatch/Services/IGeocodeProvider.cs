namespace SolarWatch.Services
{
    public interface IGeocodeProvider
    {
        string GetGeocode(string city);
    }
}
