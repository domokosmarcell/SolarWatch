﻿using SolarWatch.Models;
using SolarWatch.Data;
using Microsoft.EntityFrameworkCore;

namespace SolarWatch.Services.Repositories
{
    public class SolarTimeInfoRepository : ISolarTimeInfoRepository
    {
        private readonly SolarWatchContext _context;
        public SolarTimeInfoRepository(SolarWatchContext solarWatchContext)
        {
            _context = solarWatchContext;
        }
        public async Task<SolarTimeInfo> Add(SolarTimeInfo solarTimeInfo)
        {
            await _context.SolarTimeInfos.AddAsync(solarTimeInfo);
            await _context.SaveChangesAsync();
            return solarTimeInfo;
        }

        public async Task<SolarTimeInfo?> GetByCityDateAndTzid(City city, DateOnly date, string tzid)
        {
            return await _context.SolarTimeInfos.Include(x => x.City).FirstOrDefaultAsync(x => 
            
                string.IsNullOrEmpty(tzid)
                    ? x.City.Id == city.Id && x.Date == date && x.Tzid == "UTC"
                    : x.City.Id == city.Id && x.Date == date && x.Tzid == tzid
            );
        }

        public async Task<SolarTimeInfo> Update(SolarTimeInfo solarTimeInfo)
        {
            _context.SolarTimeInfos.Update(solarTimeInfo);
            await _context.SaveChangesAsync();
            return solarTimeInfo; 
        }
        public async Task<int> Delete(int id)
        {
            var solarTimeInfo = await _context.SolarTimeInfos.FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception("SolartimeInfo you want to delete cannot be found!!");
            _context.SolarTimeInfos.Remove(solarTimeInfo);
            await _context.SaveChangesAsync();
            return solarTimeInfo.Id;
        }

        public async Task<IEnumerable<SolarTimeInfo>?> GetAll()
        {
            var solarTimeInfos = await _context.SolarTimeInfos.Include(x => x.City).ToArrayAsync();
            return solarTimeInfos;
        }
    }
}
