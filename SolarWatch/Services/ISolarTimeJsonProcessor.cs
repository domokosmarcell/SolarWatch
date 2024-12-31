namespace SolarWatch.Services
{
    public interface ISolarTimeJsonProcessor
    {
        (TimeOnly, TimeOnly) ProcessSolarTimeInfo(string solarTimeInfo);
    }
}
