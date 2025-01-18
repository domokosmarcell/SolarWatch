using SolarWatch.Models;

namespace SolarWatch.Services.Repositories
{
    public interface ISolarTimeInfoRepository
    {
        SolarTimeInfo GetByCityAndDate(City city, DateOnly date);
        int Add(SolarTimeInfo solarTimeInfo);
    }
}
