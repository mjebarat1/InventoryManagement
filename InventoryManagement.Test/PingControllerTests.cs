using InventoryManagement.Api.Controllers;
using InventoryManagement.Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Test;

public sealed class PingControllerTests
{
    [Fact]
    public void Get_ReturnsOkResponse()
    {
        var controller = new PingController();

        var result = controller.Get();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<PingResponse>(okResult.Value);
        Assert.Equal("ok", response.Status);
    }
}
