using Microsoft.EntityFrameworkCore;
using SolarWatch.Models;
using Microsoft.Extensions.Configuration;

namespace SolarWatch.Data;

public class SolarWatchContext : DbContext
{
    private IConfiguration _configuration;
    public SolarWatchContext (DbContextOptions<SolarWatchContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }
    public DbSet<City> Cities { get; set; }
    public DbSet<SolarTimeInfo> SolarTimeInfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("SolarWatchDatabase"));
    }

}