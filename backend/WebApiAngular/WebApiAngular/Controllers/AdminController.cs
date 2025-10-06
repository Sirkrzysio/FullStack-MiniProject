using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApiAngular.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        // ✅ Endpoint tylko dla admina
        [HttpGet("secret")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetSecret()
        {
            return Ok("🔒 Only admins can see this");
        }
    }
}