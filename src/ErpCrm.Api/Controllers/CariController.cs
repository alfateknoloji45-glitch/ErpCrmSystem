using Microsoft.AspNetCore.Mvc;

namespace ErpCrm.Api.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class CariController : ControllerBase {
        [HttpGet]
        public IActionResult GetAll() {
            // Logic to get all Cari
            return Ok();
        }
    }
}