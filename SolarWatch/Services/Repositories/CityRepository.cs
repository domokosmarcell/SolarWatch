using SolarWatch.Data;
using SolarWatch.Models;
using Microsoft.EntityFrameworkCore;

namespace SolarWatch.Services.Repositories
{
    public class CityRepository : ICityRepository
    {
        private SolarWatchContext _context;
        public CityRepository(SolarWatchContext solarWatchContext)
        {
            _context = solarWatchContext;
        }
        public async Task Add(City city)
        {
            await _context.AddAsync(city);
            await _context.SaveChangesAsync();
        }

        public async Task<City?> GetByName(string city)
        {
            string[] cityInfo = (string[])SplitString(city);
            City? foundCity = null;
            if (cityInfo.Count() == 2)
            {
                foundCity = await _context.Cities.FirstOrDefaultAsync(x =>
                (
                    x.Name == cityInfo[0] && x.Country == cityInfo[1]
                ));
            }
            else if (cityInfo.Count() == 3) 
            {
                foundCity = await _context.Cities.FirstOrDefaultAsync(x =>
                (
                    x.Name == cityInfo[0] && x.State == cityInfo[1] && x.Country == cityInfo[2]
                ));
            }
            return foundCity;
        }

        private IEnumerable<string> SplitString(string city)
        {
            string[] cityInfo = city.Split(',');
            return cityInfo;
        }
    }
}
