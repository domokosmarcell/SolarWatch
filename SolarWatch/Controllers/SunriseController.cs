using Microsoft.AspNetCore.Mvc;

namespace SolarWatch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SunriseController : ControllerBase
    {
        private ILogger<SunriseController> _logger;
        public SunriseController(ILogger<SunriseController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Get")]
        public ActionResult<DateTime> Get(DateOnly date, string city) 
        {
            throw new NotImplementedException();
        }
    }
}
