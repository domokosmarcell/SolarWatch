using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.Services;

namespace SolarWatch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SunriseController : ControllerBase
    {
        private readonly ILogger<SunriseController> _logger;
        private readonly IGeocodeProvider _geocodeProvider;
        private readonly IGeocodeJsonProcessor _geocodeJsonProcessor;
        public SunriseController(ILogger<SunriseController> logger, IGeocodeProvider geocodeProvider, IGeocodeJsonProcessor geocodeJsonProcessor)
        {
            _logger = logger;
            _geocodeProvider = geocodeProvider;
            _geocodeJsonProcessor = geocodeJsonProcessor;
        }

        [HttpGet("Get")]
        public ActionResult<TimeOnly> Get([Required] DateOnly date, [Required] string city) 
        {
            throw new NotImplementedException();
        }
    }
}
