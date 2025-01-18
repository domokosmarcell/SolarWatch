using SolarWatch.Models;

namespace SolarWatch.Services.Repositories
{
    public interface ICityRepository
    {
        City GetByName(string city);
        int Add(City city);
    }
}
