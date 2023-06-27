using Microsoft.AspNetCore.Mvc;

namespace GeneriAPI.WEB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericAPIController : ControllerBase
    {
        [HttpGet]
        public ActionResult<int> GetSomething()
        {
            return Ok(0);
        }
    }
}
