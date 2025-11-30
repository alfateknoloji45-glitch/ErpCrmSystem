using Microsoft.AspNetCore.Mvc;

namespace ErpCrm.Api.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase {
        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request) {
            // Authentication logic
            return Ok();
        }
    }
}