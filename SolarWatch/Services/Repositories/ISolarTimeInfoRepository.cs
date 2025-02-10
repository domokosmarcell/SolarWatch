using SolarWatch.Models;

namespace SolarWatch.Services.Repositories
{
    public interface ISolarTimeInfoRepository
    {
        Task<SolarTimeInfo?> GetByCityAndDate(City city, DateOnly date);
        Task<SolarTimeInfo> Add(SolarTimeInfo solarTimeInfo);
        Task<SolarTimeInfo> Update(SolarTimeInfo solarTimeInfo);
    }
}
