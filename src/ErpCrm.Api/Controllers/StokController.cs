using Microsoft.AspNetCore.Mvc;

namespace ErpCrm.Api.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class StokController : ControllerBase {
        [HttpGet]
        public IActionResult GetAll() {
            // Logic to get all Stok
            return Ok();
        }
    }
}