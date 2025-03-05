using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.Models;
using SolarWatch.Services.ApiProviders;
using SolarWatch.Services.JsonProcessors;
using SolarWatch.Services.Repositories;

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
        private readonly ICityRepository _cityRepository;
        private readonly ISolarTimeInfoRepository _solarTimeInfoRepository;
        public SunriseController(ILogger<SunriseController> logger, IGeocodeProvider geocodeProvider,
            IGeocodeJsonProcessor geocodeJsonProcessor, ISolarTimeProvider solarTimeProvider, ISolarTimeJsonProcessor solarTimeJsonProcessor,
            ISolarTimeInfoRepository solarTimeInfoRepository, ICityRepository cityRepository)
        {
            _logger = logger;
            _geocodeProvider = geocodeProvider;
            _geocodeJsonProcessor = geocodeJsonProcessor;
            _solarTimeProvider = solarTimeProvider;
            _solarTimeJsonProcessor = solarTimeJsonProcessor;
            _solarTimeInfoRepository = solarTimeInfoRepository;
            _cityRepository = cityRepository;
        }

        [HttpGet("Get")]
        [Authorize(Roles = "User,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TimeOnly>> GetSunriseTime([Required] DateOnly date, [Required] string city, string? tzid) 
        {
            try
            {
                City? cityInfo = await _cityRepository.GetByName(city);
                if (cityInfo == null)
                {
                    cityInfo = _geocodeJsonProcessor.ProcessGeocodeInfo(await _geocodeProvider.GetGeocode(city), city);
                    await _cityRepository.Add(cityInfo);
                }
                SolarTimeInfo? solarTimeInfo = await _solarTimeInfoRepository.GetByCityDateAndTzid(cityInfo, date, tzid);
                if (solarTimeInfo == null)
                {
                    solarTimeInfo = _solarTimeJsonProcessor.ProcessSolarTimeInfo(await _solarTimeProvider.GetSolarTimes(cityInfo.Latitude, cityInfo.Longitude, date, tzid), date, cityInfo);
                    await _solarTimeInfoRepository.Add(solarTimeInfo);
                }
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
