using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.Services.JsonProcessors;
using SolarWatch.Services;

namespace SolarWatch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SunsetController : ControllerBase
    {
        private ILogger<SunsetController> _logger;
        private readonly IGeocodeProvider _geocodeProvider;
        private readonly IGeocodeJsonProcessor _geocodeJsonProcessor;
        private readonly ISolarTimeProvider _solarTimeProvider;
        private readonly ISolarTimeJsonProcessor _solarTimeJsonProcessor;
        public SunsetController(ILogger<SunsetController> logger, IGeocodeProvider geocodeProvider,
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
                (float lat, float lon) geocode = _geocodeJsonProcessor.ProcessGeocodeInfo(_geocodeProvider.GetGeocode(city));
                (TimeOnly sunrise, TimeOnly sunset) solarTimes = _solarTimeJsonProcessor.ProcessSolarTimeInfo(_solarTimeProvider.GetSolarTimes(geocode.lat, geocode.lon, date, tzid));
                _logger.LogInformation("Getting sunset time was successful!");
                return Ok(solarTimes.sunset);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting the sunset time for the specified city at the provided date!");
                return BadRequest("Error getting the sunset time for the specified city at the provided date!");
            }
        }
    }
}
