using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.DTOs;
using SolarWatch.Models;
using SolarWatch.Services.Repositories;

namespace SolarWatch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class SolarTimeInfoController : ControllerBase
    {
        private readonly ILogger<SolarTimeInfoController> _logger;
        private readonly ISolarTimeInfoRepository _solarTimeInfoRepository;
        private readonly ICityRepository _cityRepository;

        public SolarTimeInfoController(ILogger<SolarTimeInfoController> logger, ISolarTimeInfoRepository solarTimeInfoRepository, ICityRepository cityRepository)
        {
            _logger = logger;
            _solarTimeInfoRepository = solarTimeInfoRepository;
            _cityRepository = cityRepository;
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<SolarTimeInfo>?>> GetAll()
        {
            try
            {
                var solarTimeInfos = await _solarTimeInfoRepository.GetAll();
                return Ok(solarTimeInfos);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error during retrieving all {nameof(SolarTimeInfo)} object from the database!!");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("Add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SolarTimeInfo>> Add(SolarTimeInfoDTO solarTimeInfoDTO)
        {
            try
            {
                var cityAsNavigationProp = await _cityRepository.GetByName(solarTimeInfoDTO.CityName) ?? throw new Exception($"The city that you want to create a {nameof(SolarTimeInfo)} object for does not exist in the database!!");
                SolarTimeInfo solarTimeInfo = new()
                {
                    City = cityAsNavigationProp,
                    Date = solarTimeInfoDTO.Date,
                    Sunrise = solarTimeInfoDTO.Sunrise,
                    Sunset = solarTimeInfoDTO.Sunset,
                    Tzid = solarTimeInfoDTO.Tzid
                };
                SolarTimeInfo addedSolarTimeInfo = await _solarTimeInfoRepository.Add(solarTimeInfo);
                return CreatedAtAction(nameof(Add), addedSolarTimeInfo);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error during adding the provided {nameof(SolarTimeInfo)} object to the database!!");
                return BadRequest(e.Message);
            }
        }

        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SolarTimeInfo>> Update(SolarTimeInfoDTO solarTimeInfoDTO)
        {
            try
            {
                var cityAsNavigationProp = await _cityRepository.GetByName(solarTimeInfoDTO.CityName) ?? throw new Exception($"The city that you want to attach to the specified {nameof(SolarTimeInfo)} object does not exist in the database!!");
                SolarTimeInfo solarTimeInfo = new()
                {
                    Id = solarTimeInfoDTO.Id,
                    City = cityAsNavigationProp,
                    Date = solarTimeInfoDTO.Date,
                    Sunrise = solarTimeInfoDTO.Sunrise,
                    Sunset = solarTimeInfoDTO.Sunset,
                    Tzid = solarTimeInfoDTO.Tzid
                };
                SolarTimeInfo updatedSolarTimeInfo = await _solarTimeInfoRepository.Update(solarTimeInfo);
                return Ok(updatedSolarTimeInfo);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error during updating the specified {nameof(SolarTimeInfo)} entity!!");
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> Delete(int id)
        {
            try
            {
                int idOfDeletedSolarTimeInfo = await _solarTimeInfoRepository.Delete(id);
                return Ok(idOfDeletedSolarTimeInfo);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error during deleting the specified {nameof(SolarTimeInfo)} object from the database!!");
                return BadRequest(e.Message);
            }
        }
    }
}
