using SolarWatch.Models;

namespace SolarWatch.Services.Repositories
{
    public interface ISolarTimeInfoRepository
    {
        Task<SolarTimeInfo?> GetByCityDateAndTzid(City city, DateOnly date, string tzid);
        Task<SolarTimeInfo> Add(SolarTimeInfo solarTimeInfo);
        Task<SolarTimeInfo> Update(SolarTimeInfo solarTimeInfo);
    }
}
