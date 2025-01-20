using SolarWatch.Models;

namespace SolarWatch.Services.Repositories
{
    public interface ISolarTimeInfoRepository
    {
        Task<SolarTimeInfo?> GetByCityAndDate(City city, DateOnly date);
        Task Add(SolarTimeInfo solarTimeInfo);
    }
}
