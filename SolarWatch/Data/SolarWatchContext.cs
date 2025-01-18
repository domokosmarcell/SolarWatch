using Microsoft.EntityFrameworkCore;
using SolarWatch.Models;
using Microsoft.Extensions.Configuration;

namespace SolarWatch.Data;

public class SolarWatchContext : DbContext
{
    public SolarWatchContext (DbContextOptions<SolarWatchContext> options) : base(options)
    {
    }
    public DbSet<City> Cities { get; set; }
    public DbSet<SolarTimeInfo> SolarTimeInfos { get; set; }

}