using Microsoft.AspNetCore.Mvc;

namespace SolarWatch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SunsetController : ControllerBase
    {
        private ILogger<SunsetController> _logger;
        public SunsetController(ILogger<SunsetController> logger)
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
