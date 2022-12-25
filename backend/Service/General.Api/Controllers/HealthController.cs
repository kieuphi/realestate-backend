using Microsoft.AspNetCore.Mvc;

namespace General.Api.Controllers
{
    public class HealthController : ApiController
    {
        [HttpGet]
        public ActionResult Health()
        {
            return Ok();
        }
    }
}
