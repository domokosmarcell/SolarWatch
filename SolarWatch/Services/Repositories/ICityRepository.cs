using SolarWatch.Models;

namespace SolarWatch.Services.Repositories
{
    public interface ICityRepository
    {
        Task<City?> GetByName(string city);
        Task<City> Add(City city);
    }
}
