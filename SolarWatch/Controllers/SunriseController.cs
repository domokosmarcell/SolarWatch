using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.Models;
using SolarWatch.Services.ApiProviders;
using SolarWatch.Services.JsonProcessors;

namespace SolarWatch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SunriseController : ControllerBase
    {
        private readonly ILogger<SunriseController> _logger;
        private readonly IGeocodeProvider _geocodeProvider;
        private readonly IGeocodeJsonProcessor _geocodeJsonProcessor;
        private readonly ISolarTimeProvider _solarTimeProvider;
        private readonly ISolarTimeJsonProcessor _solarTimeJsonProcessor;
        public SunriseController(ILogger<SunriseController> logger, IGeocodeProvider geocodeProvider,
            IGeocodeJsonProcessor geocodeJsonProcessor, ISolarTimeProvider solarTimeProvider, ISolarTimeJsonProcessor solarTimeJsonProcessor)
        {
            _logger = logger;
            _geocodeProvider = geocodeProvider;
            _geocodeJsonProcessor = geocodeJsonProcessor;
            _solarTimeProvider = solarTimeProvider;
            _solarTimeJsonProcessor = solarTimeJsonProcessor;
        }

        [HttpGet("Get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TimeOnly>> GetSunriseTime([Required] DateOnly date, [Required] string city, string? tzid) 
        {
            try
            {
                City cityInfo = _geocodeJsonProcessor.ProcessGeocodeInfo(await _geocodeProvider.GetGeocode(city), city);
                SolarTimeInfo solarTimeInfo = _solarTimeJsonProcessor.ProcessSolarTimeInfo(await _solarTimeProvider.GetSolarTimes(cityInfo.Latitude, cityInfo.Longitude, date, tzid), date, cityInfo);
                _logger.LogInformation("Getting sunrise time was successful!");
                return Ok(solarTimeInfo.Sunrise);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting the sunrise time for the specified city at the provided date!");
                return BadRequest(e.Message);
            }
        }
    }
}
