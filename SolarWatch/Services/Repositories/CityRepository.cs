using SolarWatch.Data;
using SolarWatch.Models;
using Microsoft.EntityFrameworkCore;

namespace SolarWatch.Services.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly SolarWatchContext _context;
        public CityRepository(SolarWatchContext solarWatchContext)
        {
            _context = solarWatchContext;
        }
        public async Task<City> Add(City city)
        {
            await _context.Cities.AddAsync(city);
            await _context.SaveChangesAsync();
            return city;
        }

        public async Task<int> Delete(int id)
        {
            var city = await _context.Cities.FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception("The city you want to delete cannot be found!!");
            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();
            return city.Id;
        }

        public async Task<IEnumerable<City>?> GetAll()
        {
            var cities = await _context.Cities.ToArrayAsync();
            return cities;
        }

        public async Task<City?> GetByName(string city)
        {
            string[] cityInfo = city.Split(',');
            City? foundCity = null;
            if (cityInfo.Length == 2)
            {
                foundCity = await _context.Cities.FirstOrDefaultAsync(x =>
                (
                    x.Name == cityInfo[0] && x.Country == cityInfo[1]
                ));
            }
            else if (cityInfo.Length == 3) 
            {
                foundCity = await _context.Cities.FirstOrDefaultAsync(x =>
                (
                    x.Name == cityInfo[0] && x.State == cityInfo[1] && x.Country == cityInfo[2]
                ));
            }
            return foundCity;
        }

        public async Task<City> Update(City city)
        {
            _context.Cities.Update(city);
            await _context.SaveChangesAsync();
            return city;
        }
    }
}
