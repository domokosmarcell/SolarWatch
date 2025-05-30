using SolarWatch.Services.ApiProviders;
using SolarWatch.Services.HttpClientWrapper;
using SolarWatch.Services.JsonProcessors;
using SolarWatch.Data;
using Microsoft.EntityFrameworkCore;
using SolarWatch.Models;
using SolarWatch.Services.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using SolarWatch.Services.Authentication;

namespace SolarWatch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration["ConnectionStrings:SolarWatchDatabase"];
            var validIssuer = builder.Configuration["JwtTokenValidators:ValidIssuer"];
            var validAudience = builder.Configuration["JwtTokenValidators:ValidAudience"];
            var secretKey = builder.Configuration["JwtTokenValidators:SecretKey"];

            AddServices(builder);
            ConfigureSwagger(builder);
            AddDbContexts(builder, connectionString);
            AddAuthentication(builder, validIssuer, validAudience, secretKey);
            AddIdentity(builder);

            var app = builder.Build();

            ApplyMigrations(app);

            CreateRoles(app);

            SeedDb(app);


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();



            static void AddServices(WebApplicationBuilder builder)
            {
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSingleton<IGeocodeProvider, GeocodeProvider>();
                builder.Services.AddSingleton<ISolarTimeProvider, SolarTimeProvider>();
                builder.Services.AddSingleton<IGeocodeJsonProcessor, GeocodeJsonProcessor>();
                builder.Services.AddSingleton<ISolarTimeJsonProcessor, SolarTimeJsonProcessor>();
                builder.Services.AddSingleton<IHttpClient, HttpClientWrapper>();
                builder.Services.AddScoped<ICityRepository, CityRepository>();
                builder.Services.AddScoped<ISolarTimeInfoRepository, SolarTimeInfoRepository>();
                builder.Services.AddScoped<IAuthService, AuthService>();
                builder.Services.AddScoped<ITokenService, TokenService>();
                builder.Services.AddScoped<AuthenticationSeeder>();
            }

            static void ConfigureSwagger(WebApplicationBuilder builder)
            {
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "SolarWatch API", Version = "v1" });
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                        {
                            In = ParameterLocation.Header,
                            Description = "Please enter a valid token",
                            Name = "Authorization",
                            Type = SecuritySchemeType.Http,
                            BearerFormat = "JWT",
                            Scheme = "Bearer"
                        });
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type=ReferenceType.SecurityScheme,
                                        Id="Bearer"
                                    }
                                },
                                new string[]{}
                            }
                        });
                });
            }

            static void AddDbContexts(WebApplicationBuilder builder, string connectionString)
            {
                builder.Services.AddDbContext<SolarWatchContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });
                builder.Services.AddDbContext<UsersContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });
            }

            static void AddAuthentication(WebApplicationBuilder builder, string validIssuer, string validAudience, string secretKey)
            {
                builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = validIssuer,
                        ValidAudience = validAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(secretKey)
                        ),
                    };
                });
            }

            static void AddIdentity(WebApplicationBuilder builder)
            {
                builder.Services
                .AddIdentityCore<IdentityUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<UsersContext>();
            }

            static void ApplyMigrations(WebApplication webApplication)
            {
                using var scope = webApplication.Services.CreateScope();
                var solarWatchContext = scope.ServiceProvider.GetService<SolarWatchContext>();
                var usersContext = scope.ServiceProvider.GetService<UsersContext>();
                if (solarWatchContext.Database.GetPendingMigrations().Any())
                {
                    solarWatchContext.Database.Migrate();
                }
                if (usersContext.Database.GetPendingMigrations().Any())
                {
                    usersContext.Database.Migrate();
                }
            }

            static void SeedDb(WebApplication webApplication)
            {
                using (var scope = webApplication.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<SolarWatchContext>();

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

            static void CreateRoles(WebApplication webApplication)
            {
                using var scope = webApplication.Services.CreateScope();
                var authenticationSeeder = scope.ServiceProvider.GetRequiredService<AuthenticationSeeder>();
                authenticationSeeder.AddRoles();
                authenticationSeeder.AddAdmin();
            }


        }
    }


}
