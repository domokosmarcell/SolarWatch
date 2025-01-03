using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
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
        public ActionResult<TimeOnly> Get([Required] DateOnly date, [Required] string city, string? tzid) 
        {
            try
            {
                (float lat, float lon) geocode = _geocodeJsonProcessor.ProcessGeocodeInfo(_geocodeProvider.GetGeocode(city), city);
                (TimeOnly sunrise, TimeOnly sunset) solarTimes = _solarTimeJsonProcessor.ProcessSolarTimeInfo(_solarTimeProvider.GetSolarTimes(geocode.lat, geocode.lon, date, tzid), date);
                _logger.LogInformation("Getting sunrise time was successful!");
                return Ok(solarTimes.sunrise);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting the sunrise time for the specified city at the provided date!");
                return BadRequest(e.Message);
            }
        }
    }
}
