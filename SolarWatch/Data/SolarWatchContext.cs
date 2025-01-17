using Microsoft.EntityFrameworkCore;
using SolarWatch.Models;

namespace SolarWatch.Data;

public class SolarWatchContext : DbContext
{
    DbSet<City> Cities { get; set; }
    DbSet<SolarTimeInfo> SolarTimeInfos { get; set; }


    
}