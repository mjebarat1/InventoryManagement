using InventoryManagement.Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class PingController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PingResponse>(StatusCodes.Status200OK)]
    public ActionResult<PingResponse> Get()
    {
        return Ok(new PingResponse("ok"));
    }
}
