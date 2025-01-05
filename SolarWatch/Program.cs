using SolarWatch.Services.ApiProviders;
using SolarWatch.Services.JsonProcessors;
using SolarWatch.Services.WebClientWrapper;

namespace SolarWatch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IGeocodeProvider, GeocodeProvider>();
            builder.Services.AddSingleton<ISolarTimeProvider, SolarTimeProvider>();
            builder.Services.AddSingleton<IGeocodeJsonProcessor, GeocodeJsonProcessor>();
            builder.Services.AddSingleton<ISolarTimeJsonProcessor, SolarTimeJsonProcessor>();
            builder.Services.AddSingleton<IWebClient, WebClientWrapper>();

            var app = builder.Build();

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
