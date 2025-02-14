using SolarWatch.Services.ApiProviders;
using SolarWatch.Services.HttpClientWrapper;
using SolarWatch.Services.JsonProcessors;
using SolarWatch.Data;
using Microsoft.EntityFrameworkCore;
using SolarWatch.Models;
using SolarWatch.Services.Repositories;

namespace SolarWatch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("SolarWatchDatabase");

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IGeocodeProvider, GeocodeProvider>();
            builder.Services.AddSingleton<ISolarTimeProvider, SolarTimeProvider>();
            builder.Services.AddSingleton<IGeocodeJsonProcessor, GeocodeJsonProcessor>();
            builder.Services.AddSingleton<ISolarTimeJsonProcessor, SolarTimeJsonProcessor>();
            builder.Services.AddSingleton<IHttpClient, HttpClientWrapper>();
            builder.Services.AddScoped<ICityRepository, CityRepository>();
            builder.Services.AddScoped<ISolarTimeInfoRepository, SolarTimeInfoRepository>();
            builder.Services.AddDbContext<SolarWatchContext>(options => 
            {
                options.UseSqlServer(connectionString);
            });

            var app = builder.Build();

            SeedDb(app);

            static void SeedDb(WebApplication webApplication)
            {
                using (var scope = webApplication.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<SolarWatchContext>();
                    if (!context.Database.GetPendingMigrations().Any())
                    {
                        context.Database.Migrate();
                    }

                    if (context.Cities.Any() || context.SolarTimeInfos.Any())
                    {
                        return;
                    }

                    var paks = new City { Name = "Paks", Latitude = 46.6229468f, Longitude = 18.8589364f, Country = "HU" };
                    var osaka = new City { Name = "Osaka", Latitude = 34.6937569f, Longitude = 135.5014539f, State = "Osaka Prefecture", Country = "JP"};
                    var vancouver = new City { Name = "Vancouver", Latitude = 49.2608724f, Longitude = -123.113952f, State = "British Columbia", Country = "CA"};
                    context.Cities.AddRange(paks, osaka, vancouver);

                    var paksSolarTime = new SolarTimeInfo { City = paks, Date = new DateOnly(2023, 11, 23), Sunrise = new TimeOnly(6, 55, 49), Sunset = new TimeOnly(16, 5, 54), Tzid = "Europe/Budapest"};
                    var osakaSolarTime = new SolarTimeInfo { City = osaka, Date = new DateOnly(1999, 7, 11), Sunrise = new TimeOnly(19, 51, 47), Sunset = new TimeOnly(10, 14, 57), Tzid = "UTC"};
                    var vancouverSolarTime = new SolarTimeInfo { City = vancouver, Date = new DateOnly(2011, 9, 1), Sunrise = new TimeOnly(6, 27, 7), Sunset = new TimeOnly(19, 57, 46), Tzid = "America/Vancouver"};
                    context.SolarTimeInfos.AddRange(paksSolarTime, osakaSolarTime, vancouverSolarTime);

                    context.SaveChanges();
                }
            }


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
