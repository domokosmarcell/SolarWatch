using SolarWatch.Models;

namespace SolarWatch.Services.JsonProcessors
{
    public interface ISolarTimeJsonProcessor
    {
        SolarTimeInfo ProcessSolarTimeInfo(string solarTimeInfo, DateOnly date, City city);
    }
}
