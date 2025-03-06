using SolarWatch.Models;

namespace SolarWatch.Services.Repositories
{
    public interface ISolarTimeInfoRepository
    {
        Task<IEnumerable<SolarTimeInfo>?> GetAll();
        Task<SolarTimeInfo?> GetByCityDateAndTzid(City city, DateOnly date, string tzid);
        Task<SolarTimeInfo> Add(SolarTimeInfo solarTimeInfo);
        Task<SolarTimeInfo> Update(SolarTimeInfo solarTimeInfo);
        Task<int> Delete(int id);
    }
}
