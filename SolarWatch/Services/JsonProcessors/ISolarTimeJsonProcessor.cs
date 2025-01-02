namespace SolarWatch.Services.JsonProcessors
{
    public interface ISolarTimeJsonProcessor
    {
        (TimeOnly, TimeOnly) ProcessSolarTimeInfo(string solarTimeInfo, DateOnly date);
    }
}
