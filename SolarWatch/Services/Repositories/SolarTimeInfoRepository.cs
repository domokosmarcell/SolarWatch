using SolarWatch.Models;

namespace SolarWatch.Services.Repositories
{
    public class SolarTimeInfoRepository : ISolarTimeInfoRepository
    {
        public Task Add(SolarTimeInfo solarTimeInfo)
        {
            throw new NotImplementedException();
        }

        public Task<SolarTimeInfo?> GetByCityAndDate(City city, DateOnly date)
        {
            throw new NotImplementedException();
        }
    }
}
