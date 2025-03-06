using SolarWatch.Models;

namespace SolarWatch.Services.Repositories
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>?> GetAll();
        Task<City?> GetByName(string city);
        Task<City> Add(City city);
        Task<City> Update(City city);
        Task<int> Delete(int id);
    }
}
