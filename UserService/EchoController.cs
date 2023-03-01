using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserService
{
    [ApiController]
    [Authorize(Policy = "APIAccess")]
    [Route("[controller]")]
    public class EchoController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("This is private echo endpoint, try post request method");
        }
        
        [HttpPost]
        public IActionResult Post(object body)
        {
            return Ok(body);
        }
    }
}