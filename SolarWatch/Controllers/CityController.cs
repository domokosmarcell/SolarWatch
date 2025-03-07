using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.Models;
using SolarWatch.Services.Repositories;

namespace SolarWatch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class CityController : ControllerBase
    {
        private readonly ILogger<CityController> _logger;
        private readonly ICityRepository _cityRepository;

        public CityController(ILogger<CityController> logger, ICityRepository cityRepository)
        {
            _logger = logger;
            _cityRepository = cityRepository;
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<City>?>> GetAll()
        {
            try
            {
                var cities = await _cityRepository.GetAll();
                return Ok(cities);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error during retrieving all {nameof(City)} object from the database!!");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("Add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<City>> Add(City city)
        {
            try
            {
                City addedCity = await _cityRepository.Add(city);
                return CreatedAtAction(nameof(Add), addedCity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error during adding the provided {nameof(City)} object to the database!!");
                return BadRequest(e.Message);
            }
        }

        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<City>> Update(City city)
        {
            try
            {
                City updatedCityEntity = await _cityRepository.Update(city);
                return Ok(updatedCityEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error during updating the specified {nameof(City)} entity!!");
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
                int idOfDeletedCity = await _cityRepository.Delete(id);
                return Ok(idOfDeletedCity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error during deleting the specified {nameof(City)} object from the database!!");
                return BadRequest(e.Message);
            }
        }
    }
}
