using InventoryManagement.Api.DTO;
using InventoryManagement.Application.Articles.CreateFoodArticle;
using InventoryManagement.Application.Ports.In;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly ICreateFoodArticleUseCase _createFoodArticleUseCase;

        public ArticlesController(ICreateFoodArticleUseCase createFoodArticleUseCase)
        {
            _createFoodArticleUseCase = createFoodArticleUseCase;
        }

        [HttpPost("food")]
        public async Task<IActionResult> CreateFoodArticle([FromBody] CreateFoodArticleRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateFoodArticleCommand(
                request.Reference,
                request.Name,
                request.PriceExcludingTax,
                request.SaleModes);

            var result = await _createFoodArticleUseCase.ExecuteAsync(
                command,
                cancellationToken);

            //return CreatedAtAction(
            //    nameof(CreateFoodArticle),
            //    new { id = result.ArticleId },
            //    result);

            return StatusCode(StatusCodes.Status201Created, new
            {
                id = result.ArticleId
            });
        }
    }
}
